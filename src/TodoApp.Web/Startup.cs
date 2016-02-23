using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.FileProviders;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using ServiceStartup = TodoApp.Service.Startup;

namespace TodoApp.Web
{
    public class Startup
    {
        private ServiceStartup ServiceStartup { get; } = new ServiceStartup();

        public void ConfigureServices(IServiceCollection services)
        {
            ServiceStartup.ConfigureServices(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment environment, IApplicationEnvironment runtime, IServiceProvider serviceProvider)
        {
            app.UseIISPlatformHandler();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(runtime.ApplicationBasePath, "node_modules")),
                RequestPath = new PathString("/node_modules"),
                ContentTypeProvider = new FileExtensionContentTypeProvider()
            });

            app.Map(
                new PathString("/api"),
                subApp => ServiceStartup.Configure(subApp));

            app.Run(context =>
            {
                context.Response.StatusCode = 404;
                return Task.FromResult(0);
            });
        }

        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
