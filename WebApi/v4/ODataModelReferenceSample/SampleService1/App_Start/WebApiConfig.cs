using System.Collections.Generic;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Extensions;
using System.Xml;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Library;
using SampleService2.Models;

namespace SampleService1
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var model2 = EdmxReader.Parse(XmlReader.Create("http://localhost:9091/odata/$metadata"));

            var model = new EdmModel();
            model.AddReferencedModel(model2);

            var reference = new EdmReference("http://localhost:9091/odata/$metadata");
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

            config.MapODataServiceRoute("odata", "odata", model);
        }
    }
}
