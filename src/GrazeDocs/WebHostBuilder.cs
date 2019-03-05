using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace GrazeDocs
{
    public class WebHostBuilder
    {
        public static IWebHostBuilder CreateWebHostBuilder(Options options)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console();

            if (options.VerboseLogging)
            {
                loggerConfiguration.MinimumLevel.Debug();
                loggerConfiguration.MinimumLevel.Override("Microsoft", LogEventLevel.Information);
                loggerConfiguration.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information);
            }
            else
            {
                loggerConfiguration.MinimumLevel.Information();
                loggerConfiguration.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
                loggerConfiguration.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);
            }

            Log.Logger = loggerConfiguration
                .CreateLogger();

            var result = WebHost.CreateDefaultBuilder()
                .SuppressStatusMessages(true)
                .UseUrls(options.LivePreviewUrl)
                .UseSerilog()
                .UseStartup<Startup>();

            result.ConfigureServices(x =>
            {
                x.AddSingleton<Options>(options);
            });

            return result;
        }
    }
}
