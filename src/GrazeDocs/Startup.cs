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
            services.AddSignalR();
            services.AddHostedService<DocumentationWatcherService>();
            services.AddHostedService<DocumentCreationService>();
            services.AddSingleton((provider) =>
            {
                var configuration = provider.GetService<Configuration>();
                var graze = new graze.Core(new graze.Core.Parameters(configuration.ThemeFolder, configuration.LivePreviewPublishFolder, true, null, null, false, configuration.ConfigurationFile, configuration.AssetsFolder, null, 4, configuration.GrazeBinFolder));
                return graze;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Configuration configuration)
        {
            app.UseMiddleware<InjectLivePreviewMiddleware>();

            var options = new FileServerOptions()
            {
                EnableDirectoryBrowsing = true,
                FileProvider = new PhysicalFileProvider(configuration.LivePreviewPublishFolder),
            };

            app.UseFileServer(options);

            app.UseSignalR(routes =>
            {
                routes.MapHub<LivePreviewHub>("/.livepreview");
            });
        }
    }
}
