using System.Linq;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using TodoApp.Service.Repositories;

namespace TodoApp.Service
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                var jsonOutputFormatter = options.OutputFormatters.OfType<JsonOutputFormatter>().First();
                jsonOutputFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });
            services.AddSingleton<ITodoRepository, TodoRepository>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
        }
    }
}
