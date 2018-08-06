// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;

namespace ODataUntypedSample
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
            app.UseMvc(routeBuilder
                => routeBuilder.MapODataServiceRoute("odata", "odata", GetModel()));
        }

        private static IEdmModel GetModel()
        {
            EdmModel model = new EdmModel();

            // Create and add product entity type.
            EdmEntityType product = new EdmEntityType("NS", "Product");
            product.AddKeys(product.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            product.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            product.AddStructuralProperty("Price", EdmPrimitiveTypeKind.Double);
            model.AddElement(product);

            // Create and add category entity type.
            EdmEntityType category = new EdmEntityType("NS", "Category");
            category.AddKeys(category.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            category.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            model.AddElement(category);

            // Set navigation from product to category.
            EdmNavigationPropertyInfo propertyInfo = new EdmNavigationPropertyInfo();
            propertyInfo.Name = "Category";
            propertyInfo.TargetMultiplicity = EdmMultiplicity.One;
            propertyInfo.Target = category;
            EdmNavigationProperty productCategory = product.AddUnidirectionalNavigation(propertyInfo);

            // Create and add entity container.
            EdmEntityContainer container = new EdmEntityContainer("NS", "DefaultContainer");
            model.AddElement(container);

            // Create and add entity set for product and category.
            EdmEntitySet products = container.AddEntitySet("Products", product);
            EdmEntitySet categories = container.AddEntitySet("Categories", category);
            products.AddNavigationTarget(productCategory, categories);

            return model;
        }
    }
}
