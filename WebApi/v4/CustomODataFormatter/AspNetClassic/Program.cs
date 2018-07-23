// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.Owin.Hosting;
using Owin;

namespace CustomODataFormatter
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:9000/";

            // Start OWIN host
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                // Create HttpCient and make a request to api/values 
                HttpClient client = new HttpClient();

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, baseAddress + "odata/Documents?search=cat");
                request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

                // set the Prefer header is necessary to return the annotations.
                request.Headers.Add("Prefer", "odata.include-annotations=*");

                var response = client.SendAsync(request).Result;

                Console.WriteLine(response);
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);

                Console.ReadLine();
            }
        }
    }

    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            IEdmModel model = GetEdmModel();
            config.MapODataServiceRoute("odata", "odata", builder => 
                builder.AddService(ServiceLifetime.Singleton, sp => model)
                       // create the formatters with the custom serializer provider and use them in the configuration.
                       .AddService<ODataSerializerProvider, CustomODataSerializerProvider>(ServiceLifetime.Singleton)
                       .AddService<IEnumerable<IODataRoutingConvention>>(ServiceLifetime.Singleton, sp =>
                              ODataRoutingConventions.CreateDefaultWithAttributeRouting("odata", config)));

            appBuilder.UseWebApi(config);
        }

        private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Document>("Documents");
            return builder.GetEdmModel();
        }
    }
}