// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using BasicWebApiSample.Models;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;

namespace BasicWebApiSample
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
            services.AddDbContext<CustomerOrderContext>(opt => opt.UseInMemoryDatabase("CustomerOrdersList"));
            services.AddOData();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var model = GetEdmModel(app.ApplicationServices);

            app.UseMvc(builder =>
            {
                builder.Filter().Expand().Select().Count().OrderBy().MaxTop(null);
                builder.MapODataServiceRoute("odata1", "odata", model);
                builder.MapODataServiceRoute("odata2", "inmem", model);
            });
        }

        private static IEdmModel GetEdmModel(IServiceProvider servicePrivider)
        {
            var b = new ODataConventionModelBuilder(servicePrivider);
            b.EntitySet<Customer>("Customers").EntityType.Property(p=>p.Name).IsConcurrencyToken();
            b.EntitySet<Order>("Orders");
            return b.GetEdmModel();
        }
    }
}
