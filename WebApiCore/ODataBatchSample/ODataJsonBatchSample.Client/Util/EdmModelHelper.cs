using Microsoft.OData.Edm;

namespace ODataJsonBatchSample.Client.Util
{
    class EdmModelHelper
    {
        public static IEdmModel GetModel()
        {
            EdmModel model = new EdmModel();
            EdmEntityContainer container = new EdmEntityContainer("ns", "container");
            model.AddElement(container);

            EdmEntityType customer = new EdmEntityType("ns", "Customer");
            customer.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            EdmStructuralProperty key = customer.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            customer.AddKeys(key);
            model.AddElement(customer);

            container.AddEntitySet("Customers", customer);

            return model;
        }
    }
}
