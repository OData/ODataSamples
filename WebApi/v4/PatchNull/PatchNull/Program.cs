﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
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

            dynamic delta = new JObject();
            delta.FirstName = "def";
            delta.LastName = null;
            var json = JsonConvert.SerializeObject(delta);
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
        }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class CustomersController : ODataController
    {
        private static readonly Customer[] _customers =
            { new Customer { Id = 0, FirstName = "abc", LastName = "xyz" } };

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
    }
}
