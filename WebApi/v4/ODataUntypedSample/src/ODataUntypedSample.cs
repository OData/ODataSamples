using System;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.OData.Extensions;
using Microsoft.OData.Edm;
#if WEBAPIODATA_6X
#else
using Microsoft.OData.Edm.Library;
#endif
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using Owin;
using System.Xml;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ODataUntypedSample
{
    public class ODataUntypedSample
    {
        private static HttpClient client = new HttpClient();
#if WEBAPIODATA_6X
        private const string ServiceUrl = "http://localhost:54321";
#else
        private const string ServiceUrl = "http://localhost:12345";
#endif
        public static IEdmModel Model = GetEdmModel();

        public static void Main(string[] args)
        {
            using (WebApp.Start(ServiceUrl, Configuration))
            {
                Console.WriteLine("Server is listening at {0}", ServiceUrl);

                RunSample();

                Console.WriteLine("Press any key to continue . . .");
                Console.ReadKey();
            }
        }

        public static void Configuration(IAppBuilder builder)
        {
            HttpConfiguration configuration = new HttpConfiguration();
#if WEBAPIODATA_6X
            configuration.Count().Filter().OrderBy().Expand().MaxTop(null);
#endif
            configuration.MapODataServiceRoute("odata", "odata", Model);
            builder.UseWebApi(configuration);
        }

        public static IEdmModel GetEdmModel()
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

        public static void RunSample()
        {
            Console.WriteLine("1. Get Metadata.");
            GetMetadata();

            Console.WriteLine("\n2. Get Entity Set.");
            GetEntitySet();

            Console.WriteLine("\n3. Get Entity.");
            GetEntity(5);

            Console.WriteLine("\n4. Get Property From Entity.");
            GetPropertyFromEntity(8);

            Console.WriteLine("\n5. Post Entity.");
            PostEntity(123);
        }

        public static void GetMetadata()
        {
            HttpResponseMessage response = client.GetAsync(ServiceUrl + "/odata/$metadata").Result;
            PrintResponse(response);
        }

        public static void GetEntitySet()
        {
            HttpResponseMessage response = client.GetAsync(ServiceUrl + "/odata/Products?$filter=Id eq 1").Result;
            PrintResponse(response);
        }

        public static void GetEntity(int id)
        {
            HttpResponseMessage response = client.GetAsync(ServiceUrl + String.Format("/odata/Products({0})", id)).Result;
            PrintResponse(response);
        }

        public static void GetPropertyFromEntity(int id)
        {
            HttpResponseMessage response = client.GetAsync(ServiceUrl + String.Format("/odata/Products({0})/Category", id)).Result;
            PrintResponse(response);
        }

        public static void PostEntity(int id)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, ServiceUrl + "/odata/Products");
            request.Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    Id = id,
                    Name = "Product " + id,
                    Price = 123.45,
                    Category = new
                    {
                        Id = id % 5,
                        Name = "Category " + (id % 5)
                    }
                }),
                Encoding.Default,
                "application/json");

            HttpResponseMessage response = client.SendAsync(request).Result;
            PrintResponse(response);
        }

        public static void PrintResponse(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            Console.WriteLine("Response:");
            Console.WriteLine(response);

#if WEBAPIODATA_6X
            if (response.Content != null)
            {
                string payload = response.Content.ReadAsStringAsync().Result;

                if (response.Content.Headers.ContentType.MediaType.Contains("xml"))
                {
                    Console.WriteLine(FormatXml(payload));
                }
                else if (response.Content.Headers.ContentType.MediaType.Contains("json"))
                {
                    JObject jobj = JObject.Parse(payload);
                    Console.WriteLine(jobj);
                }
            }
#else
            if (response.Content != null)
            {
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }
#endif
        }

        private static string FormatXml(string source)
        {
            StringBuilder sb = new StringBuilder();
            XmlTextWriter writer = null;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(source);

                writer = new XmlTextWriter(new StringWriter(sb));
                writer.Formatting = System.Xml.Formatting.Indented;

                doc.WriteTo(writer);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }

            return sb.ToString();
        }
    }
}