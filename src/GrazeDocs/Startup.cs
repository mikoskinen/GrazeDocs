using GrazeDocs.LivePreview;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace GrazeDocs
{
    public partial class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddRouting();
            services.AddSignalR();
            services.AddHostedService<DocumentationWatcherService>();
            services.AddHostedService<DocumentCreationService>();

            services.AddSingleton(provider =>
            {
                var configuration = provider.GetService<GrazeDocsOptions>();
                var graze = new graze.Core(new graze.Core.Parameters(configuration.ThemeFolder,
                    configuration.PublishFolder, true, null, null, false, configuration.ConfigurationFile,
                    configuration.AssetsFolder, null, 4, configuration.GrazeBinFolder));
                return graze;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, GrazeDocsOptions options)
        {
            app.UseMiddleware<LivePreviewMiddleware>();

            var fileOptions = new FileServerOptions()
            {
                EnableDirectoryBrowsing = true, FileProvider = new PhysicalFileProvider(options.PublishFolder),
            };

            app.UseFileServer(fileOptions);
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<LivePreviewHub>("/.livepreview");
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
