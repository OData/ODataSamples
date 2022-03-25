using System.Linq;
using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Lab02Sample03.Models;

namespace Lab02Sample03
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOData();
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseODataBatching();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints
                    .Select().Filter().OrderBy().Count().Expand().MaxTop(50)
                    .SetUrlKeyDelimiter(ODataUrlKeyDelimiter.Parentheses);

                var defaultBatchHandler = new DefaultODataBatchHandler();
                defaultBatchHandler.MessageQuotas.MaxNestingDepth = 2;
                defaultBatchHandler.MessageQuotas.MaxOperationsPerChangeset = 10;

                endpoints.MapODataRoute("odata", "odata", GetEdmModel(), defaultBatchHandler);
            });
        }

        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Book>("Books");
            builder.EntitySet<Author>("Authors");
            builder.EntitySet<Publisher>("Publishers");
            builder.Function("ReturnAllForKidsBooks").ReturnsFromEntitySet<Book>("Books");
            builder.EntityType<Book>().Collection
                .Function("MostRecent")
                .Returns<int>();
            builder.EntityType<Book>()
                .Action("Rate")
                .Parameter<int>("Rating");
            var model = builder.GetEdmModel();
            return model;
        }
    }
}
