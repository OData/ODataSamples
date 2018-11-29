// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ODataComplexTypeInheritanceSample.Test
{
    class Program
    {
        private static readonly string _baseAddress = "https://localhost:44304/";
        private static readonly HttpClient _httpClient = new HttpClient();

        // namespace string for the CLR types in the models.
        private static readonly string _namespace = "ODataComplexTypeInheritanceSample.Models";

        private const string _shapeObjectForAction = @"
{
    'shape':
    {
        '@odata.type':'#ODataComplexTypeInheritanceSample.Models.Polygon',
        'HasBorder':true,
        'Vertexes':[
            {
              'X':0,'Y':0
            },
            {
              'X':2,'Y':0
            },
            {
              'X':2,'Y':2
            },
            {
              'X':0,'Y':2
            }
        ]
    }
}";
        private const string _shapeObjectForPost = @"
{

    '@odata.type':'#ODataComplexTypeInheritanceSample.Models.Polygon',
    'HasBorder':true,
    'Vertexes':[
        {
          'X':0,'Y':0
        },
        {
          'X':2,'Y':0
        },
        {
          'X':2,'Y':2
        },
        {
          'X':0,'Y':2
        }
    ]
}";
        static void Main(string[] args)
        {
            Console.WriteLine("Listening on " + _baseAddress);
            string requestUri = "";
            HttpResponseMessage response = null;

            // The complex type Shape is an abstract type, in the EDM model, its IsAbstract is true.
            // The complex type Circle and Polygon derive from Shape and
            // the complex type Rectangle derives from Polygon.
            requestUri = _baseAddress + "/odata/$metadata";
            Comment("GET " + requestUri);
            response = Get(requestUri);
            Comment(response);

            // The property DefaultShape in Windows is declared as Shape, and in the instance Windows(1)
            // it is actually a Polygon.
            // The property OptionalShapes is a collection of Shape, in the instance the 3 types of Shape
            // are included.
            requestUri = _baseAddress + "/odata/Windows(1)";
            Comment("GET " + requestUri);
            response = Get(requestUri);
            Comment(response);

            // Get a property defined in the derived Complex type.
            requestUri = _baseAddress + "/odata/Windows(1)/CurrentShape/ODataComplexTypeInheritanceSample.Models.Circle/Radius";
            Comment("GET " + requestUri);
            response = Get(requestUri);
            Comment(response);

            // Function that returns a base complex type
            requestUri = _baseAddress + "/odata/Windows(1)/GetTheLastOptionalShape()";
            Comment("GET " + requestUri);
            response = Get(requestUri);
            Comment(response);

            // Action that takes in a base complex type
            requestUri = _baseAddress + "/odata/Windows(1)/AddOptionalShape";
            Comment("POST " + requestUri);
            response = ActionCall(requestUri, _shapeObjectForAction);
            Comment(response);

            // Post request that add a base complex type to a collection
            requestUri = _baseAddress + "/odata/Windows(1)/OptionalShapes";
            Comment("POST " + requestUri);
            response = ActionCall(requestUri, _shapeObjectForPost);
            Comment(response);

            // This method illustrate how to add an entity which contains derived complex type.
            requestUri = _baseAddress + "/odata/Windows";
            Comment("POST " + requestUri);
            response = Post(requestUri);
            Comment(response);

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private static HttpResponseMessage Get(string requestUri)
        {
            HttpResponseMessage response = _httpClient.GetAsync(requestUri).Result;
            response.EnsureSuccessStatusCode();
            return response;
        }

        private static HttpResponseMessage Post(string requestUri)
        {
            string content = @"{
    'Id':0,
    'Name':'Name4',
    'CurrentShape':
    {
        '@odata.type':'#ODataComplexTypeInheritanceSample.Circle',
        'Radius':10,
        'Center':{'X':1,'Y':2},
        'HasBorder':true
    },
    'OptionalShapes':
    [
        {
            '@odata.type':'#ODataComplexTypeInheritanceSample.Polygon',
            'HasBorder':true,
            'Vertexes':
            [
                {
                  'X':0,'Y':0
                },
                {
                  'X':2,'Y':0
                },
                {
                  'X':2,'Y':2
                }
           ]
        }
    ]
}";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Content = new StringContent(content);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            HttpResponseMessage response = _httpClient.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            return response;
        }

        private static HttpResponseMessage ActionCall(string requestUri, string content)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Content = new StringContent(content);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            HttpResponseMessage response = _httpClient.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            return response;
        }

        private static void Comment(string message)
        {
            Console.WriteLine(message);
        }

        private static void Comment(HttpResponseMessage response)
        {
            Console.WriteLine(response);
            string result = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(result);
        }
    }
}
