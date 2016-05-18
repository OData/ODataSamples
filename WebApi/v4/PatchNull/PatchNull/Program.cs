using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PatchNull
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new HttpConfiguration();
            var builder = new ODataConventionModelBuilder(config);
            builder.EntitySet<Customer>("Customers");
            var model = builder.GetEdmModel();
            config.MapODataServiceRoute("api", "api", model);
            var server = new HttpServer(config);
            var client = new HttpClient(server);

            Console.WriteLine("=============Patch entity with null: \n");
            dynamic delta = new JObject();
            delta.FirstName = "def";
            delta.LastName = null;
            dynamic address = new JObject();
            address.Region = null;
            address.Street = "Lianhua";
            delta.Address = address;
            string json = JsonConvert.SerializeObject(delta);
            var content = new StringContent(json, Encoding.Default, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), "http://localhost/api/Customers(0)")
            {
                Content = content,
            };

            var response = client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(result);
            var customer = JsonConvert.DeserializeObject<Customer>(result);
            Debug.Assert(customer.LastName == null);
            Debug.Assert(customer.Address.Region == null);

            Console.WriteLine("\n=============Patch complex property: \n");
            // Patch a complex type property is supported: See https://github.com/OData/WebApi/issues/135
            address = new JObject();
            address.Street = "MY";
            address.City = "Paris";
            address.Region = null;
            json = JsonConvert.SerializeObject(address);

            int a = json.IndexOf("{", StringComparison.Ordinal);
            json = json.Insert(a + 1, "\"@odata.type\":\"#PatchNull.Address\",");

            content = new StringContent(json, Encoding.Default, "application/json");
            request = new HttpRequestMessage(new HttpMethod("PATCH"), "http://localhost/api/Customers(1)/Address")
            {
                Content = content,
            };

            response = client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var payload = response.Content.ReadAsAsync<JObject>().Result;
            Console.WriteLine(payload);
        }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address Address { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
    }

    public class CustomersController : ODataController
    {
        private static readonly Customer[] _customers =
        {
            new Customer
            {
                Id = 0,
                FirstName = "abc",
                LastName = "xyz",
                Address = new Address { Region = "CN", City= "Shanghai", Street = "Zixing" }
            },
            new Customer
            {
                Id = 1,
                FirstName = "def",
                LastName = "uvw",
                Address = new Address { Region = "US", City= "Redmond", Street = "One Microsoft Rd" }
            }
        };

        [AcceptVerbs("PATCH")]
        public IHttpActionResult Patch(int key, Delta<Customer> delta)
        {
            var customer = _customers.SingleOrDefault(c => c.Id == key);
            if (customer == null)
            {
                return NotFound();
            }

            delta.Patch(customer);
            return Ok(customer);
        }

        [HttpPatch]
        public IHttpActionResult PatchToAddress(int key, Delta<Address> delta)
        {
            var customer = _customers.SingleOrDefault(c => c.Id == key);
            if (customer == null)
            {
                return NotFound();
            }

            delta.Patch(customer.Address);
            return Ok(customer.Address);
        }
    }
}
 