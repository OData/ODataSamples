// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using ODataFunctionSample.Models;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;

namespace ODataFunctionSample.AspNetCore
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

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseHttpsRedirection();
            app.UseMvc(appBuilder =>
            {
                appBuilder.MapODataServiceRoute("odata", "odata", GetEdmModel());
            });
        }

        // Builds the EDM model for the OData service, including the OData action definitions.
        private static IEdmModel GetEdmModel()
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder();

            builder.EntitySet<Product>("Products");

            var productType = builder.EntityType<Product>();

            // Function bound to a collection
            // Returns the most expensive product, a single entity
            productType.Collection
                .Function("MostExpensive")
                .Returns<double>();

            // Function bound to a collection
            // Returns the top 10 product, a collection
            productType.Collection
                .Function("Top10")
                .ReturnsCollectionFromEntitySet<Product>("Products");

            // Function bound to a single entity
            // Returns the instance's price rank among all products
            productType
                .Function("GetPriceRank")
                .Returns<int>();

            // Function bound to a single entity
            // Accept a string as parameter and return a double
            // This function calculate the general sales tax base on the
            // state
            productType
                .Function("CalculateGeneralSalesTax")
                .Returns<double>()
                .Parameter<string>("state");

            // Unbound Function
            builder.Function("GetSalesTaxRate")
                .Returns<double>()
                .Parameter<string>("state");

            return builder.GetEdmModel();
        }
    }
}
