using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using Microsoft.OData.Edm;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json.Linq;
using Owin;

namespace ODataReferentialConstraintSample
{
    public class Program
    {
        private static readonly string _baseAddress = string.Format("http://{0}:12345", Environment.MachineName);
        private static readonly HttpClient _httpClient = new HttpClient();

        public static void Main(string[] args)
        {
            _httpClient.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
            using (WebApp.Start(_baseAddress, Configuration))
            {
                Console.WriteLine("Listening on " + _baseAddress);

                // The referential constraint is present on metadata document
                Comment("GET ~/$metadata");
                HttpResponseMessage response = QueryMetadata();
                string metadata = response.Content.ReadAsStringAsync().Result;
                Comment(response.ToString());
                Comment(metadata);

                // Call an action to generate the resource.
                Comment("POST ~/ResetDataSource");
                response = ResetDataSource();
                Comment(response);

                // Get a customer which id is 2
                Comment("GET ~/Customers(2)");
                response = QueryCustomer();
                Comment(response);

                // Get a order which has a foreign key to customers(2)
                Comment("GET ~/Orders(5)");
                response = QueryOrder();
                Comment(response);

                // Delete customers(2) 
                Comment("DELETE ~/Customers(2)");
                response = DeleteCustomer();
                Comment(response);

                // Can't get customers(2) 
                Comment("GET ~/Customers(2)");
                response = QueryCustomer();
                Comment(response);

                // Can't get orders(5)
                Comment("GET ~/Orders(5)");
                response = QueryOrder();
                Comment(response);

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        public static void Configuration(IAppBuilder builder)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.MapODataServiceRoute(routeName: "OData", routePrefix: "odata", model: GetModel());
            builder.UseWebApi(config);
        }

        public static HttpResponseMessage QueryMetadata()
        {
            string requestUri = _baseAddress + "/odata/$metadata";
            HttpResponseMessage response = _httpClient.GetAsync(requestUri).Result;

            response.EnsureSuccessStatusCode();
            return response;
        }

        public static HttpResponseMessage ResetDataSource()
        {
            string requestUri = _baseAddress + "/odata/ResetDataSource";

            HttpResponseMessage response = _httpClient.PostAsync(requestUri, null).Result;
            response.EnsureSuccessStatusCode();
            return response;
        }

        public static HttpResponseMessage QueryCustomer()
        {
            string requestUri = _baseAddress + "/odata/Customers(2)";

            HttpResponseMessage response = _httpClient.GetAsync(requestUri).Result;
            return response;
        }

        public static HttpResponseMessage QueryOrder()
        {
            string requestUri = _baseAddress + "/odata/Orders(5)";

            HttpResponseMessage response = _httpClient.GetAsync(requestUri).Result;
            return response;
        }

        public static HttpResponseMessage DeleteCustomer()
        {
            string requestUri = _baseAddress + "/odata/Customers(2)";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            HttpResponseMessage response = _httpClient.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            return response;
        }

        private static IEdmModel GetModel()
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Customer>("Customers");
            builder.EntitySet<Order>("Orders");
            builder.Action("ResetDataSource");

            builder.Namespace = typeof(Customer).Namespace;

            return builder.GetEdmModel();
        }

        private static void Comment(string message)
        {
            Console.WriteLine();
            Console.WriteLine(message);
        }

        private static void Comment(HttpResponseMessage response)
        {
            Console.WriteLine(response);
            JObject result = response.Content.ReadAsAsync<JObject>().Result;
            Console.WriteLine(result);
        }
    }
}
