// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;

namespace ODataUntypedSample.Client
{
    class Program
    {
        private static HttpClient client = new HttpClient();
        private const string ServiceUrl = "https://localhost:44379";

        static void Main(string[] args)
        {
            RunSample();

            Console.WriteLine("Press any key to continue . . .");
            Console.ReadKey();
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

            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            HttpResponseMessage response = client.SendAsync(request).Result;

            PrintResponse(response);
        }

        public static void PrintResponse(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            Console.WriteLine("Response:");
            Console.WriteLine(response);

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
