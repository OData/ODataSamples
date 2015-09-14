using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.OData.Edm.Vocabularies.V1;
using ODataSamples.Common.Model;

namespace ODataSamples.Edm
{
    class Program
    {
        static void Main(string[] args)
        {
            ReferentialConstraintDemo();
            EnumMemberExpressionDemo();
            CustomTermDemo();
            MutualReferenceByCodeDemo();
            MutualReferenceByEdmxDemo();
            EdmReadAnnotationDemo();
            EdmWriteAnnotationDemo();
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
            model.SetNamespaceAlias("ns", "Alias1");
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

        private static void MutualReferenceByCodeDemo()
        {
            Console.WriteLine("MutualReferenceByCodeDemo");

            var subModel1 = new EdmModel();
            var complex1 = new EdmComplexType("NS1", "Complex1");
            subModel1.AddElement(complex1);
            var reference1 = new EdmReference("http://model2");
            reference1.AddInclude(new EdmInclude("Alias2", "NS2"));
            var references1 = new List<IEdmReference> { reference1 };
            subModel1.SetEdmReferences(references1);

            var subModel2 = new EdmModel();
            var complex2 = new EdmComplexType("NS2", "Complex2");
            subModel2.AddElement(complex2);
            var reference2 = new EdmReference("http://model1");
            reference2.AddInclude(new EdmInclude("Alias1", "NS1"));
            var references2 = new List<IEdmReference> { reference2 };
            subModel2.SetEdmReferences(references2);

            complex1.AddStructuralProperty("Prop", new EdmComplexTypeReference(complex2, true));
            complex2.AddStructuralProperty("Prop", new EdmComplexTypeReference(complex1, true));

            var mainModel = new EdmModel();
            var complex3 = new EdmComplexType("NS", "Complex3");
            mainModel.AddElement(complex3);
            complex3.AddStructuralProperty("Prop1", new EdmComplexTypeReference(complex1, true));
            complex3.AddStructuralProperty("Prop2", new EdmComplexTypeReference(complex2, true));
            var references3 = new List<IEdmReference> { reference1, reference2 };
            mainModel.SetEdmReferences(references3);

            ShowModel(mainModel);
            ShowModel(subModel1);
            ShowModel(subModel2);
        }

        private static void MutualReferenceByEdmxDemo()
        {
            Console.WriteLine("MutualReferenceByEdmxDemo");

            const string mainEdmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://model2"">
    <edmx:Include Namespace=""NS2"" Alias=""Alias2"" />
  </edmx:Reference>
  <edmx:Reference Uri=""http://model1"">
    <edmx:Include Namespace=""NS1"" Alias=""Alias1"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""Complex3"">
        <Property Name=""Prop1"" Type=""NS1.Complex1"" />
        <Property Name=""Prop2"" Type=""NS2.Complex2"" />
      </ComplexType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            const string edmx1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://model2"">
    <edmx:Include Namespace=""NS2"" Alias=""Alias2"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""Complex1"">
        <Property Name=""Prop"" Type=""NS2.Complex2"" />
      </ComplexType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            const string edmx2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://model1"">
    <edmx:Include Namespace=""NS1"" Alias=""Alias1"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""NS2"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""Complex2"">
        <Property Name=""Prop"" Type=""NS1.Complex1"" />
      </ComplexType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            if (!EdmxReader.TryParse(XmlReader.Create(new StringReader(mainEdmx)), (uri) =>
            {
                if (string.Equals(uri.AbsoluteUri, "http://model1/"))
                {
                    return XmlReader.Create(new StringReader(edmx1));
                }

                if (string.Equals(uri.AbsoluteUri, "http://model2/"))
                {
                    return XmlReader.Create(new StringReader(edmx2));
                }

                throw new Exception("invalid url");
            }, out model, out errors))
            {
                throw new Exception("bad model");
            }
        }

        private static void EdmReadAnnotationDemo()
        {
            Console.WriteLine("EdmReadAnnotationDemo");

            var annotationModel = new AnnotationModel();
            var model = annotationModel.Model;
            var person = (IEdmEntityType)model.FindType("TestNS.Person");
            var annotation = model.FindVocabularyAnnotations<IEdmValueAnnotation>(person, "TestNS.OutColor").First();
            var memberExp = (IEdmEnumMemberExpression)annotation.Value;
            foreach (var member in memberExp.EnumMembers)
            {
                Console.WriteLine(member.Name);
            }
        }

        private static void EdmWriteAnnotationDemo()
        {
            Console.WriteLine("EdmWriteAnnotationDemo");

            var model = new EdmModel();

            var mail = new EdmEntityType("ns", "Mail");
            model.AddElement(mail);
            mail.AddKeys(mail.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));

            var person = new EdmEntityType("ns", "Person");
            model.AddElement(person);
            person.AddKeys(person.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            var mails = person.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                ContainsTarget = true,
                Name = "Mails",
                TargetMultiplicity = EdmMultiplicity.Many,
                Target = mail,
            });

            var ann1 = new EdmAnnotation(mails, CoreVocabularyModel.DescriptionTerm, new EdmStringConstant("test1"));
            ann1.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(ann1);

            var container = new EdmEntityContainer("ns", "container");
            model.AddElement(container);
            var people = container.AddEntitySet("People", person);
            var ann2 = new EdmAnnotation(people, CoreVocabularyModel.DescriptionTerm, new EdmStringConstant("test2"));
            model.AddVocabularyAnnotation(ann2);

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
