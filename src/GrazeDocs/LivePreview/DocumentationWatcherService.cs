using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GrazeDocs
{
    public class DocumentationWatcherService : BackgroundService
    {
        private DateTime _lastChanged = DateTime.MinValue;
        private static FileSystemWatcher _fileWatcher = null;

        public static BlockingCollection<string> ChangedFiles = new BlockingCollection<string>(1);
        private readonly ILogger<DocumentationWatcherService> _logger;
        private readonly Options _configuration;

        public DocumentationWatcherService(ILogger<DocumentationWatcherService> logger, Options configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        private void AddEventSubscriptions(FileSystemWatcher fileWatcher, WatcherChangeTypes changeTypes)
        {
            if (changeTypes.HasFlag(WatcherChangeTypes.Changed))
            {
                fileWatcher.Changed += OnFileChanged;
            }

            if (changeTypes.HasFlag(WatcherChangeTypes.Created))
            {
                fileWatcher.Created += OnFileChanged;
            }

            if (changeTypes.HasFlag(WatcherChangeTypes.Deleted))
            {
                fileWatcher.Deleted += OnFileChanged;
            }

            if (changeTypes.HasFlag(WatcherChangeTypes.Renamed))
            {
                fileWatcher.Renamed += OnFileChanged;
            }
        }

        protected void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if ((DateTime.Now - _lastChanged) < TimeSpan.FromSeconds(1))
            {
                return;
            }

            _lastChanged = DateTime.Now;
            ChangedFiles.TryAdd(e.FullPath, 1);

            _logger.LogTrace("Detected changed file {FullPath}", e.FullPath);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var watchedFolder = Path.GetDirectoryName(_configuration.ConfigurationFile);
            _logger.LogDebug("Watching changes in {watchedFolder}", watchedFolder);

            _fileWatcher = new FileSystemWatcher(watchedFolder, "*.md*")
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
            };

            _fileWatcher.NotifyFilter = NotifyFilters.LastWrite;

            AddEventSubscriptions(_fileWatcher, WatcherChangeTypes.All);

            return Task.CompletedTask;
        }
    }
}
