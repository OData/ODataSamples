using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm;
using ODataModelAliasingSample.AspNetCore.Model;

namespace ODataModelAliasingSample
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddDbContext<CustomersContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ModelAliasingCustomerContext")));
            Thread.Sleep(15 * 1000);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc(routeBuilder =>
            {
                routeBuilder.Select().Filter().Expand().OrderBy().MaxTop(100).Count();
                routeBuilder.MapODataServiceRoute("odata", "odata", GetEdmModel());
            });
        }

        // Builds the EDM model for the OData service.
        private static IEdmModel GetEdmModel()
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder();
            EntitySetConfiguration<CustomerDto> customers = builder.EntitySet<CustomerDto>("Customers");
            customers.EntityType.HasKey(entity => entity.Id);

            EntitySetConfiguration<OrderDto> orders = builder.EntitySet<OrderDto>("Orders");
            orders.EntityType.Name = "Order";
            orders.EntityType.Property(p => p.Total).Name = "Check";
            return builder.GetEdmModel();
        }
    }
}
