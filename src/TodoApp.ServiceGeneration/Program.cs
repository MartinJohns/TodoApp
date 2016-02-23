using System;
using System.IO;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using TodoApp.Service;

namespace TodoApp.ServiceGeneration
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                Generate<Startup>(
                    @"..\TodoApp.Web\wwwroot\services\todoService.g.ts",
                    @"..\TodoApp.Web\wwwroot\interfaces.g.ts");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error during service generation.");
                Console.Error.WriteLine(ex.ToString());
                return -1;
            }
        }

        public static void Generate<TStartup>(string servicePath, string interfacesPath)
            where TStartup : class
        {
            var server = new WebHostBuilder()
                .UseStartup<TStartup>()
                .Build();

            var applicationPath = server.ApplicationServices.GetService<IApplicationEnvironment>().ApplicationBasePath;

            var relativeInterfacesPath = GetRelativePath(
                Path.Combine(applicationPath, servicePath),
                Path.Combine(applicationPath, interfacesPath));

            var apiDescriptionProvider = server.ApplicationServices.GetService<IApiDescriptionGroupCollectionProvider>();
            var serviceGeneration = new ServiceGeneration(apiDescriptionProvider);
            serviceGeneration.Generate(relativeInterfacesPath);

            WriteIfUnchanged(servicePath, serviceGeneration.ServiceClient);
            WriteIfUnchanged(interfacesPath, serviceGeneration.Interfaces);
        }

        private static void WriteIfUnchanged(string path, string content)
        {
            if (!File.Exists(path) || File.ReadAllText(path) != content)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, content);
                Console.WriteLine($"Written to {path}");
            }
        }

        private static string GetRelativePath(string basePath, string destPath)
        {
            if (!Path.IsPathRooted(basePath))
                throw new ArgumentException("basePath must be rooted.", nameof(basePath));

            if (!Path.IsPathRooted(destPath))
                throw new ArgumentException("destPath must be rooted.", nameof(destPath));

            var basePathUri = new Uri(basePath);
            var destPathUri = new Uri(destPath);
            var relativePathUri = basePathUri.MakeRelativeUri(destPathUri);
            var relativePath = relativePathUri.OriginalString;
            var fileExtension = Path.GetExtension(relativePath);
            var relativePathWithoutExtension = relativePath.Substring(0, relativePath.Length - fileExtension.Length);
            return relativePathWithoutExtension;
        }
    }
}
