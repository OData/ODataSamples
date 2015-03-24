using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Annotations;
using Microsoft.OData.Edm.Library.Expressions;
using Microsoft.OData.Edm.Library.Values;
using Microsoft.OData.Edm.Validation;

namespace ODataSamples.Edm
{
    class Program
    {
        static void Main(string[] args)
        {
            ReferentialConstraintDemo();
            EnumMemberExpressionDemo();
            CustomTermDemo();
        }

        private static void ReferentialConstraintDemo()
        {
            Console.WriteLine("ReferentialConstraintDemo");

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
                DependentProperties = new[] { orderCustomerId },
                PrincipalProperties = new[] { customerId }
            };
            order.AddUnidirectionalNavigation(nav);

            ShowModel(model);
        }

        private static void EnumMemberExpressionDemo()
        {
            Console.WriteLine("EnumMemberExpressionDemo");

            var model = new EdmModel();
            var personType = new EdmEntityType("TestNS", "Person");
            model.AddElement(personType);
            var pid = personType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false);
            personType.AddKeys(pid);
            personType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            var colorType = new EdmEnumType("TestNS2", "Color", true);
            model.AddElement(colorType);
            colorType.AddMember("Cyan", new EdmIntegerConstant(1));
            colorType.AddMember("Blue", new EdmIntegerConstant(2));
            var outColorTerm = new EdmTerm("TestNS", "OutColor", new EdmEnumTypeReference(colorType, true));
            model.AddElement(outColorTerm);
            var exp = new EdmEnumMemberExpression(
                new EdmEnumMember(colorType, "Blue", new EdmIntegerConstant(2))
            );

            var annotation = new EdmAnnotation(personType, outColorTerm, exp);
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.SetVocabularyAnnotation(annotation);

            ShowModel(model);

            var ann = model.FindVocabularyAnnotations<IEdmValueAnnotation>(personType, "TestNS.OutColor").First();
            var memberExp = (IEdmEnumMemberExpression)ann.Value;
            foreach (var member in memberExp.EnumMembers)
            {
                Console.WriteLine(member.Name);
            }
        }

        private static void CustomTermDemo()
        {
            Console.WriteLine("CustomTermDemo");

            var model = new EdmModel();
            var term = new EdmTerm("ns", "ErrorCodes",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false))));
            model.AddElement(term);
            var entity1 = new EdmEntityType("ns", "entity1");
            entity1.AddKeys(entity1.AddStructuralProperty("id", EdmPrimitiveTypeKind.Guid));
            model.AddElement(entity1);
            var container = new EdmEntityContainer("ns", "default");
            model.AddElement(container);
            var e1 = container.AddSingleton("E1", entity1);

            var annotation = new EdmAnnotation(e1, term,
                new EdmCollectionExpression(
                    new EdmStringConstant("Entity Not Found"),
                    new EdmStringConstant("Deleting link failed")));

            model.AddVocabularyAnnotation(annotation);

            ShowModel(model);
        }

        private static void ShowModel(IEdmModel model)
        {
            IEnumerable<EdmError> errors;

            var writer = XmlWriter.Create(Console.Out, new XmlWriterSettings { Indent = true });
            EdmxWriter.TryWriteEdmx(model, writer, EdmxTarget.OData, out errors);
            writer.Flush();

            Console.WriteLine();

            foreach (var edmError in errors)
            {
                Console.WriteLine(edmError);
            }

            /*
            model.Validate(out errors);
            foreach (var edmError in errors)
            {
                Console.WriteLine(edmError);
            }
            */
        }
    }
}
