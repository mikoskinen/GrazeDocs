using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using graze;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GrazeDocs.LivePreview
{
    public class LivePreviewService
    {
        public static async Task RunLivePreview(Options options)
        {
            var livePublishFolder = CreateLivePublishFolder();
            options.PublishFolder = livePublishFolder;

            using (var host = WebHostBuilder.CreateWebHostBuilder(options).Build())
            {
                var logger = host.Services.GetService<ILogger<Program>>();

                try
                {
                    logger.LogInformation($"Starting GrazeDocs Live Preview on the following address: {options.LivePreviewUrl}");

                    var graze = host.Services.GetService<Core>();
                    logger.LogDebug($"Creating initial live preview to {livePublishFolder}");
                    graze.Run();

                    await host.StartAsync();

                    await Task.Delay(TimeSpan.FromSeconds(1));

                    logger.LogInformation($"GrazeDocs Live Preview ready. Starting browser...");

                    OpenBrowser(options.LivePreviewUrl);

                    await host.WaitForShutdownAsync();

                    logger.LogInformation($"Closing GrazeDocs Live Preview");
                }
                catch (Exception ex)
                {
                    logger.LogCritical(ex, "Starting GrazeDocs Live Preview failed.");
                }
                finally
                {
                    if (Directory.Exists(livePublishFolder))
                    {
                        Directory.Delete(livePublishFolder, true);
                    }
                }
            }
        }

        //https://stackoverflow.com/a/38604462/66988
        public static void OpenBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}")); // Works ok on windows
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);  // Works ok on linux
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url); // Not tested
            }
            else
            {
            }
        }

        private static string CreateLivePublishFolder()
        {
            var livePublishFolder = Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(livePublishFolder);

            return livePublishFolder;
        }

    }


}
