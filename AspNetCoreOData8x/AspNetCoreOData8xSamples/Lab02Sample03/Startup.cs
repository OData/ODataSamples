using Lab02Sample03.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

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
            var defaultBatchHandler = new DefaultODataBatchHandler();
            defaultBatchHandler.MessageQuotas.MaxNestingDepth = 2;
            defaultBatchHandler.MessageQuotas.MaxOperationsPerChangeset = 10;

            services.AddControllers().AddOData(opt =>
            {
                opt.AddRouteComponents("odata", GetEdmModel(), defaultBatchHandler).Count().Filter().Expand().Select().OrderBy().SetMaxTop(5);
                opt.RouteOptions.EnableControllerNameCaseInsensitive = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseODataBatching();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Book>("books");
            builder.EntitySet<Author>("authors");
            builder.EntitySet<Publisher>("publishers");
            builder.Function("returnAllForKidsBooks").ReturnsFromEntitySet<Book>("books");
            builder.EntityType<Book>().Collection
                .Function("mostRecent")
                .Returns<string>();
            builder.EntityType<Book>()
                .Action("rate")
                .Parameter<int>("rating");
            var model = builder.GetEdmModel();
            return model;
        }
    }
}
