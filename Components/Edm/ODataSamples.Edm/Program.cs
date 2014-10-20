using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Evaluation;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Annotations;
using Microsoft.OData.Edm.Library.Expressions;
using Microsoft.OData.Edm.Library.Values;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Values;

namespace ODataSamples.Edm
{
    class Program
    {
        static void Main(string[] args)
        {
            ReferentialConstraintDemo();
        }

        private static void ReferentialConstraintDemo()
        {
            EdmModel model = new EdmModel();

            var customer = new EdmEntityType("ns", "Customer");
            model.AddElement(customer);
            var customerId = customer.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false);
            customer.AddKeys(customerId);
            var address = new EdmComplexType("ns", "Address");
            model.AddElement(address);
            var code = address.AddStructuralProperty("gid", EdmPrimitiveTypeKind.Guid);
            customer.AddStructuralProperty("addr", new EdmComplexTypeReference(address, true));

            var order = new EdmEntityType("ns", "Order");
            model.AddElement(order);
            var oId = order.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false);
            order.AddKeys(oId);

            var orderCustomerId = order.AddStructuralProperty("CustomerId", EdmPrimitiveTypeKind.Int32, false);

            var nav = new EdmNavigationPropertyInfo()
            {
                Name = "NavCustomer",
                Target = customer,
                TargetMultiplicity = EdmMultiplicity.One,
                DependentProperties = new[] {orderCustomerId},
                PrincipalProperties = new[] {customerId}
            };
            order.AddUnidirectionalNavigation(nav);

            IEnumerable<EdmError> errors;

            var writer = XmlWriter.Create(Console.Out, new XmlWriterSettings {Indent = true});
            EdmxWriter.TryWriteEdmx(model, writer, EdmxTarget.OData, out errors);
            writer.Flush();

            foreach (var edmError in errors)
            {
                Console.WriteLine(edmError);
            }

            model.Validate(out errors);
            foreach (var edmError in errors)
            {
                Console.WriteLine(edmError);
            }
        }
    }
}
