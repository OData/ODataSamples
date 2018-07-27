// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.OData.Edm;
using Microsoft.AspNet.OData.Builder;
using ODataComplexTypeInheritanceSample.Models;

namespace ODataComplexTypeInheritance
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

            app.UseHttpsRedirection();
            app.UseMvc(appBuilder =>
            {
                appBuilder.MapODataServiceRoute("odata", "odata", GetModel());
            });
        }

        private static IEdmModel GetModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            EntitySetConfiguration<Window> windows = builder.EntitySet<Window>("Windows");
            EntityTypeConfiguration<Window> window = windows.EntityType;

            // Action that takes in a base complex type.
            ActionConfiguration actionConfiguration = window.Action("AddOptionalShape");
            actionConfiguration.Parameter<Shape>("shape");
            actionConfiguration.Returns<int>(); // The number of all optional shapes

            // Function that returns a base complex type
            var functionConfiguration = window.Function("GetTheLastOptionalShape");
            functionConfiguration.Returns<Shape>();

            builder.Namespace = typeof(Window).Namespace;

            return builder.GetEdmModel();
        }
    }
}
