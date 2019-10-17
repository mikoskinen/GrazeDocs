using System;
using System.IO;
using System.Threading;
using graze;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GrazeDocs
{
    public class Publisher
    {
        public static void Publish(GrazeDocsOptions options)
        {
            using (var host = WebHostBuilder.CreateWebHostBuilder(options).Build())
            {
                var logger = host.Services.GetService<ILogger<Program>>();

                try
                {
                    var publishFolder = Path.Combine(Environment.CurrentDirectory, "publish");

                    if (Directory.Exists(publishFolder))
                    {
                        logger.LogDebug("Publish folder {folderName} exists, try to delete it first.", publishFolder);
                        Directory.Delete(publishFolder, true);

                        Thread.Sleep(TimeSpan.FromSeconds(0.3));
                    }

                    options.PublishFolder = publishFolder;

                    logger.LogDebug("Creating publish folder {folderName}.", publishFolder);
                    Directory.CreateDirectory(publishFolder);

                    var graze = host.Services.GetService<Core>();
                    logger.LogInformation($"Publish documentation to {publishFolder}");
                    graze.Run();

                    logger.LogInformation($"Publish ready");
                }
                catch (Exception ex)
                {
                    logger.LogCritical(ex, "GrazeDocs publish failed.");
                }
            }
        }
    }
}
