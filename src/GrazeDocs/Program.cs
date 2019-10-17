using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using GrazeDocs.LivePreview;
using Mono.Options;

namespace GrazeDocs
{
    public partial class Program
    {
        public static async Task Main(string[] args)
        {
            var options = new GrazeDocsOptions();
            var showHelp = false;

            var optionSet = new OptionSet
                              {
                                      {"i|init=", "Initialize GrazeDocs to {DIRECTORY}. Use . for current directory.", x => options.InitializeFolder = x},
                                      {"p|publish", "Publish documents to publish-directory.", x => options.Publish = true},
                                      {"w|watch|live", "Start GrazeDocs Live Preview.", x => options.LivePreviewEnabled = true},
                                      {"t|port=", "Live Preview HTTP {PORT}. Default is 7552.", x => options.LivePreviewUrl = $"http://localhost:{x}"},
                                      {"v|verbose", "Verbose logging.", x => options.VerboseLogging = true},
                                      { "h|?|help",  "Show help.", x => showHelp = true},
                              };

            optionSet.Parse(args);
            SetOptions(options);

            if (showHelp)
            {
                PrintHelp(optionSet);
            }
            else if (options.LivePreviewEnabled)
            {
                if (!File.Exists(options.ConfigurationFile))
                {
                    throw new Exception($"Directory {Path.GetDirectoryName(options.ConfigurationFile)} is not a valid GrazeDocs directory. Missing configuration file {options.ConfigurationFile}. Use grazedocs init to initialize the documentation directory.");
                }

                await LivePreviewService.RunLivePreview(options);
            }
            else if (!string.IsNullOrWhiteSpace(options.InitializeFolder))
            {
                var initialReadmeCreated = Initializer.Initialize(options);

                if (initialReadmeCreated)
                {
                    Console.WriteLine($"Initialized GrazeDocs. Happy documenting!");
                }
                else
                {
                    Console.WriteLine($"Initialized GrazeDocs with a readme.md. Happy documenting!");
                }
            }
            else if (options.Publish)
            {
                Publisher.Publish(options);
            }
            else if (!string.IsNullOrWhiteSpace(options.AddDocumentPath))
            {
                // Todo
            }
            else
            {
                PrintHelp(optionSet);
            }
        }


        private static void PrintHelp(OptionSet options)
        {
            Console.WriteLine("Usage: grazedocs [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");

            options.WriteOptionDescriptions(Console.Out);
        }


        private static void SetOptions(GrazeDocsOptions options)
        {
            options.GrazeBinFolder = Path.GetDirectoryName(typeof(graze.Core).Assembly.Location);
            options.PublishFolder = null;

            if (Debugger.IsAttached)
            {
                options.ThemeFolder = @"C:\dev\projects\GrazeDocs\samples\Single Page\_theme";
                options.ConfigurationFile = @"C:\dev\projects\GrazeDocs\samples\Single Page\configuration.xml";
                options.AssetsFolder = @"C:\dev\projects\GrazeDocs\samples\Single Page\_theme\assets";
            }
            else
            {
                var currentFolder = Environment.CurrentDirectory;
                options.ThemeFolder = Path.Combine(currentFolder, "_theme");
                options.ConfigurationFile = Path.Combine(currentFolder, "configuration.xml");
                options.AssetsFolder = Path.Combine(options.ThemeFolder, "assets");
            }
        }
    }
}
