using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData.Extensions;
using Microsoft.OData.Core;
using Microsoft.OData.Core.UriParser;
using Microsoft.Spatial;
using ODataSpatialSample.Models;

namespace ODataSpatialSample
{
    class Program
    {
        private static string BaseUri = "http://localhost/odata/";

        static void Main(string[] args)
        {
            // CreatePolygon();
            BuildDatabase();

            HttpClient client = GetClient();

            Query(client, "$metadata");

            Query(client, "Customers(2)");

            // only return Customers(1)(2)(3)
            Query(client, "Customers?&$filter=geo.intersects(Location, geography'SRID=4326;POLYGON((0 0, 0 3, 3 3, 3 0, 0 0))')");

            // only return Customers(1)
            Query(client, "Customers?&$filter=geo.intersects(Location, geography'SRID=4326;POLYGON((0 0, 0 1.5, 1.5 1.5, 1.5 0, 0 0))')");

            Post(client, "Customers");

            Console.ReadKey();
        }

        private static async void Query(HttpClient client, string request)
        {
            Console.WriteLine("\n**********************************\n[Request]: " + request);

            HttpResponseMessage response = await client.GetAsync(BaseUri + request);

            Console.WriteLine("[StatusCode]: " + response.StatusCode);

            if (response.Content != null)
            {
                Console.WriteLine("[Content]:\n" + await response.Content.ReadAsStringAsync());
            }
        }

        private static async void Post(HttpClient client, string requestUri)
        {
            Console.WriteLine("\n**********************************\n[Request]: POST " + requestUri);

        const string payload = @"{
  ""Location"":{
    ""type"":""Point"",""coordinates"":[
      2.0,2.0,2.0,2.0
    ],""crs"":{
      ""type"":""name"",""properties"":{
        ""name"":""EPSG:4326""
      }
    }
  },""LineString"":{
    ""type"":""LineString"",""coordinates"":[
      [
        1.0,1.0
      ],[
        3.0,3.0
      ],[
        4.0,4.0
      ],[
        0.0,0.0
      ]
    ],""crs"":{
      ""type"":""name"",""properties"":{
        ""name"":""EPSG:4326""
      }
    }
  },""Name"":""Venus""
}";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, BaseUri + requestUri);
            request.Content = new StringContent(payload);
            request.Content.Headers.ContentType = MediaTypeWithQualityHeaderValue.Parse("application/json");
            HttpResponseMessage response = client.SendAsync(request).Result;

            Console.WriteLine("[StatusCode]: " + response.StatusCode);

            if (response.Content != null)
            {
                Console.WriteLine("[Content]:\n" + await response.Content.ReadAsStringAsync());
            }
        }

        private static HttpClient GetClient()
        {
            var config = new HttpConfiguration();

            config.MapODataServiceRoute("odata", "odata", EdmModelBuilder.GetEdmModel());

            return new HttpClient(new HttpServer(config));
        }

        private static void BuildDatabase()
        {
            SpatialDataContext db = new SpatialDataContext();

            if (db.Customers.Any())
            {
                return;
            }

            string[] names = {"Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune"};

            string[] lineStrings =
            {
                "LINESTRING(1 1, 3 3)",
                "LINESTRING(1 1, 3 3, 2 4, 2 0)",
                "LINESTRING(1 1, 3 3, 2 4, 2 0, 1 1)",
                "LINESTRING(1 1, 2 4, 3 9)",
                "LINESTRING(1 1 NULL 0, 2 4 NULL 12.3, 3 9 NULL 24.5)",
                "LINESTRING(2 1, 2 3)",
                "LINESTRING(3 2, 4 6)"
            };
            var customers = Enumerable.Range(1, 7).Select(e => new Customer
            {
                Id = e,
                Name = names[e-1],
                DbLocation = DbGeography.FromText(String.Format("POINT({0} {1} {2} {3})", e, e, e, e)),
                DbLineString = DbGeography.FromText(lineStrings[e-1])
            });

            foreach (var customer in customers)
            {
                db.Customers.Add(customer);
            }

            db.SaveChanges();
        }

        private static string CreatePolygon()
        {
            /*
            GeographyPolygon polygonValue =
                GeographyFactory.Polygon()
                    .Ring(33.1, -110.0)
                    .LineTo(35.97, -110.15)
                    .LineTo(11.45, 87.75)
                    .Ring(35.97, -110)
                    .LineTo(36.97, -110.15)
                    .LineTo(45.23, 23.18)
                    .Build();*/

            GeographyPolygon polygonValue =
                GeographyFactory.Polygon()
                    .Ring(0, 0)
                    .LineTo(5, 0)
                    .LineTo(5, 5)
                    .LineTo(0, 5)
                    .Build();

            string polygonString = ODataUriUtils.ConvertToUriLiteral(polygonValue, ODataVersion.V4);

            Console.WriteLine(polygonString);

            return polygonString;
        }
    }
}
