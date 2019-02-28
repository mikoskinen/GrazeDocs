using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using graze;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GrazeDocs
{
    public class DocumentCreationService : BackgroundService
    {
        private readonly ILogger<DocumentCreationService> _logger;
        private readonly Core _graze;
        private readonly IServerAddressesFeature _serverAddressesFeature;
        private HubConnection _connection;

        public DocumentCreationService(ILogger<DocumentCreationService> logger, graze.Core graze, IServer server)
        {
            _logger = logger;
            _graze = graze;
            _serverAddressesFeature = server.Features.Get<IServerAddressesFeature>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var livePreviewNotifyUrl = $"{_serverAddressesFeature.Addresses.First()}/.livepreview";
            _logger.LogDebug("Connecting to live preview notification url {livePreviewNotifyUrl}. The url is used to push notifications to web browser", livePreviewNotifyUrl);

            _connection = new HubConnectionBuilder()
                .WithUrl(livePreviewNotifyUrl)
                .Build();

            await _connection.StartAsync(stoppingToken);

            await Task.Run(async () =>
            {
                try
                {
                    foreach (var changedFile in DocumentationWatcherService.ChangedFiles.GetConsumingEnumerable(stoppingToken))
                    {
                        try
                        {
                            _logger.LogDebug("Running live preview");
                            _graze.Run();
                            _logger.LogInformation("Live preview updated");

                            await _connection.InvokeAsync("Refresh");
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Error when updating live preview");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                        // Swallow
                    }

            }, stoppingToken);
        }
    }
}
