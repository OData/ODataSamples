// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using Microsoft.OData;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json.Linq;

namespace ODataCapabilitiesVocabularySample
{
    class Program
    {
        private static readonly string _baseAddress = string.Format("http://localhost:12345/");
        private static HttpClient _httpClient = new HttpClient();

        static void Main(string[] args)
        {
            _httpClient.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
            using (WebApp.Start<Startup>(_baseAddress))
            {
                Console.WriteLine("Listening on " + _baseAddress);

                // Query metadata without capabilities annotation
                Comment("GET ~/odata/non-cap/$metadata");
                HttpResponseMessage response = QueryMetadata("odata/non-cap");
                string metadata = response.Content.ReadAsStringAsync().Result;
                Comment(response.ToString());
                Comment(PrintXML(metadata));

                // Query metadata with capabilities annotation
                Comment("GET ~/odata/cap/$metadata");
                response = QueryMetadata("odata/cap");
                metadata = response.Content.ReadAsStringAsync().Result;
                Comment(response.ToString());
                Comment(PrintXML(metadata));

                // Query Customers without capabilities annotation
                Comment("GET ~/odata/cap/Customers");
                response = QueryCustomers("odata/non-cap");
                response.EnsureSuccessStatusCode();
                string customers = response.Content.ReadAsStringAsync().Result;
                Comment(response.ToString());
                Comment(PrintJson(customers));

                // Query Customers with capabilities annotation
                Comment("GET ~/odata/cap/Customers");
                response = QueryCustomers("odata/cap");
                response.EnsureSuccessStatusCode();
                customers = response.Content.ReadAsStringAsync().Result;
                Comment(response.ToString());
                Comment(PrintJson(customers));

                // Query Customers without capabilities annotation
                Comment("GET ~/odata/cap/Customers?$expand=Orders");
                response = QueryCustomers("odata/non-cap", "$expand=Orders");
                response.EnsureSuccessStatusCode();
                customers = response.Content.ReadAsStringAsync().Result;
                Comment(response.ToString());
                Comment(PrintJson(customers));

                // Query Customers with capabilities annotation
                Comment("GET ~/odata/cap/Customers?$expand=Orders");
                response = QueryCustomers("odata/cap", "$expand=Orders");
                if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                {
                    throw new ODataException("It should be a Bad Request!");
                }
                else
                {
                    Console.WriteLine("***A repsonse with bad request is expected***");
                }

                customers = response.Content.ReadAsStringAsync().Result;
                Comment(response.ToString());
                Comment(PrintJson(customers));

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        public static HttpResponseMessage QueryMetadata(string prefix)
        {
            string requestUri = _baseAddress + prefix + "/$metadata";
            HttpResponseMessage response = _httpClient.GetAsync(requestUri).Result;
            response.EnsureSuccessStatusCode();
            return response;
        }

        public static HttpResponseMessage QueryCustomers(string prefix, string query = null)
        {
            string requestUri = _baseAddress + prefix + "/Customers";
            if (query != null)
            {
                requestUri += "?" + query;
            }
            HttpResponseMessage response = _httpClient.GetAsync(requestUri).Result;
            return response;
        }

        private static void Comment(string message)
        {
            Console.WriteLine(message);
        }

        private static void Comment(HttpResponseMessage response)
        {
            Console.WriteLine(response);
            JObject result = response.Content.ReadAsAsync<JObject>().Result;
            Console.WriteLine(result);
        }

        public static string PrintJson(string json)
        {
            JObject obj = JObject.Parse(json);
            return obj.ToString();
        }

        public static string PrintXML(string xml)
        {
            string result = "";
            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();

            try
            {
                document.LoadXml(xml);
                writer.Formatting = Formatting.Indented;
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();
                mStream.Position = 0;
                StreamReader sReader = new StreamReader(mStream);
                string formattedXml = sReader.ReadToEnd();
                result = formattedXml;
            }
            catch (XmlException)
            {
                // Handle the exception
            }

            mStream.Close();
            writer.Close();
            return result;
        }
    }
}
