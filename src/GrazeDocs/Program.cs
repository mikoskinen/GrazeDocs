using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using graze;
using graze.common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mono.Options;
using Serilog;
using Serilog.Events;

namespace GrazeDocs
{
    public partial class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = new Configuration();
            var showHelp = false;

            var options = new OptionSet
                              {
                                      {"i|init=", "Initialize GrazeDocs to directory. Use . for current directory.", x => configuration.InitializeFolder = x},
                                      {"a|add=", "Add new document with name.", x => configuration.AddDocumentPath = x},
                                      {"w|watch|l|live", "Start GrazeDocs Live Preview.", x => configuration.LivePreviewEnabled = true},
                                      {"p|port=", "Live Preview HTTP port. Default is 7552.", x => configuration.LivePreviewUrl = $"http://localhost:{x}"},
                                      {"v", "Verbose logging.", x => configuration.VerboseLogging = true},
                                      { "h|?|help",  "Show help.", x => showHelp = true},
                              };

            options.Parse(args);

            SetConfiguration(configuration);

            if (showHelp)
            {
                PrintHelp(options);
            }
            else if (configuration.LivePreviewEnabled)
            {
                if (!File.Exists(configuration.ConfigurationFile))
                {
                    throw new Exception($"Directory {Path.GetDirectoryName(configuration.ConfigurationFile)} is not a valid GrazeDocs directory. Missing configuration file {configuration.ConfigurationFile}. Use grazedocs init to initialize the documentation directory.");
                }

                await RunLivePreview(configuration);
            }
            else if (!string.IsNullOrWhiteSpace(configuration.InitializeFolder))
            {
                Initialize(configuration);

                Console.WriteLine($"Initialized GrazeDocs. Happy documenting!");
            }
            else if (!string.IsNullOrWhiteSpace(configuration.AddDocumentPath))
            {
                if (!File.Exists(configuration.ConfigurationFile))
                {
                    throw new Exception($"Directory {Path.GetDirectoryName(configuration.ConfigurationFile)} is not a valid GrazeDocs directory. Missing configuration file {configuration.ConfigurationFile}. Use grazedocs init to initialize the documentation directory and then used grazedocs add to add a documentation page.");
                }
            }
            else
            {
                PrintHelp(options);
            }
        }

        private static void Initialize(Configuration configuration)
        {
            if (File.Exists(configuration.ConfigurationFile))
            {
                throw new Exception($"Directory {Path.GetDirectoryName(configuration.ConfigurationFile)} can not be initialized. It already contains the GrazeDocs configuration file {configuration.ConfigurationFile}.");
            }

            var folder = configuration.InitializeFolder;
            if (string.Equals(folder, "."))
            {
                folder = Environment.CurrentDirectory;
            }

            Console.WriteLine($"Initalizing GrazeDocs on {folder}");

            var projectName = "";
            while (string.IsNullOrWhiteSpace(projectName))
            {
                Console.Write("Project name: ");
                projectName = Console.ReadLine();
            }

            var themeFolder = Path.Combine(folder, "_theme");
            var configurationFile = Path.Combine(folder, "configuration.xml");

            var configurationTemplate = $@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<data>
  <site>
    <Title>{projectName}</Title>
    <Description>{projectName}: Documentation</Description>
	<Logo>/assets/logo.png</Logo>
	<Footer>Copyright 2019</Footer>
	<Navigations>
		<Navigation Url=""/"" ExtraClass=""active"">Documentation</Navigation>
	</Navigations>
	<SecondaryLinks>
		<SecondaryLink Url=""https://grazedocs.io"">Project Home</SecondaryLink>
	</SecondaryLinks>	
  </site>
  <ChildPages Location="""" DefaultPageLayoutFile=""page.cshtml"" IndexLayoutFile=""pagesindex.cshtml"" IndexFileName=""all.html"" FolderPerPage=""false"" ReadmeName=""index""
              TagsIndexLayoutFile=""tagsindex.cshtml"" TagLayoutFile=""tag.cshtml"" RelativePathPrefix=""""
			  RssGenerate=""false"" RssFeedName="""" RssUri="""" RssAuthor="""" RssDescription="""">
    <Groups>
      <Group Key="""">Home</Group>
    </Groups>
  </ChildPages>
</data>";
            Directory.CreateDirectory(themeFolder);

            var themeTemplateFolder = Path.Combine(configuration.GrazeBinFolder, "_theme");
            DirCopy.Copy(themeTemplateFolder, themeFolder);

            File.WriteAllText(configurationFile, configurationTemplate, Encoding.UTF8);
        }

        private static void PrintHelp(OptionSet options)
        {
            Console.WriteLine("Usage: grazedocs [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");

            options.WriteOptionDescriptions(Console.Out);
        }

        private static async Task RunLivePreview(Configuration configuration)
        {
            var livePublishFolder = CreateLivePublishFolder();
            SetConfiguration(configuration, livePublishFolder);

            using (var host = CreateWebHostBuilder(configuration).Build())
            {
                var logger = host.Services.GetService<ILogger<Program>>();

                try
                {
                    logger.LogInformation($"Starting GrazeDocs Live Preview on the following address: {configuration.LivePreviewUrl}");

                    var graze = host.Services.GetService<Core>();
                    logger.LogDebug($"Creating initial live preview to {livePublishFolder}");
                    graze.Run();

                    await host.StartAsync();

                    await Task.Delay(TimeSpan.FromSeconds(1));

                    OpenBrowser(configuration.LivePreviewUrl);

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

        private static string CreateLivePublishFolder()
        {
            var livePublishFolder = Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(livePublishFolder);

            return livePublishFolder;
        }

        private static void SetConfiguration(Configuration configuration)
        {
            configuration.GrazeBinFolder = Path.GetDirectoryName(typeof(graze.Core).Assembly.Location);
            configuration.LivePreviewPublishFolder = null;

            if (Debugger.IsAttached)
            {
                configuration.ThemeFolder = @"C:\dev\projects\GrazeDocs\sample\_theme";
                configuration.ConfigurationFile = @"C:\dev\projects\GrazeDocs\sample\configuration.xml";
                configuration.AssetsFolder = @"C:\dev\projects\GrazeDocs\sample\_theme\assets";
            }
            else
            {
                var currentFolder = Environment.CurrentDirectory;
                configuration.ThemeFolder = Path.Combine(currentFolder, "_theme");
                configuration.ConfigurationFile = Path.Combine(currentFolder, "configuration.xml");
                configuration.AssetsFolder = Path.Combine(configuration.ThemeFolder, "assets");
            }
        }

        private static void SetConfiguration(Configuration configuration, string livePreviewPublishFolder)
        {
            configuration.LivePreviewPublishFolder = livePreviewPublishFolder;
        }

        public static IWebHostBuilder CreateWebHostBuilder(Configuration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            var result = WebHost.CreateDefaultBuilder()
                .SuppressStatusMessages(true)
                .UseUrls(configuration.LivePreviewUrl)
                .UseSerilog()
                .UseStartup<Startup>();

            result.ConfigureServices(x =>
            {
                x.AddSingleton<Configuration>(configuration);
            });

            return result;
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

    }
}
