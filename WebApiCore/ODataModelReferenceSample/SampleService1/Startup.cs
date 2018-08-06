using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using SampleService2.Models;

namespace ODataModelReferenceSample
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
                => routeBuilder.MapODataServiceRoute("odata", "odata", GetModel())
            );
        }

        private static IEdmModel GetModel()
        {
            var model2 = CsdlReader.Parse(XmlReader.Create("https://localhost:44350/odata/$metadata"));

            var model = new EdmModel();
            model.AddReferencedModel(model2);

            var reference = new EdmReference(new Uri("https://localhost:44350/odata/$metadata"));
            reference.AddInclude(new EdmInclude("Model2", "SampleService2.Models"));
            model.SetEdmReferences(new List<IEdmReference> { reference });

            var container = new EdmEntityContainer("NS1", "Default");
            var order = model2.FindDeclaredType("SampleService2.Models.Order") as IEdmEntityType;
            model2.SetAnnotationValue<ClrTypeAnnotation>(order, new ClrTypeAnnotation(typeof(Order)));
            container.AddEntitySet("Orders", order);
            model.AddElement(container);

            var product = new EdmEntityType("NS1", "Product");
            product.AddKeys(product.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            product.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            model.AddElement(product);

            return model;
        }
    }
}
