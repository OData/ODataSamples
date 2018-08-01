// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.OData;
using ODataJsonBatchSample.Client.Util;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ODataJsonBatchSample.Client
{
    class Program
    {
        private static readonly string serviceUrl = "http://localhost:65200/odata";
        private static readonly string requestUri = string.Format("{0}/$batch", serviceUrl);
        private static readonly string absoluteUri = string.Format("{0}/customers", serviceUrl);
        private static HttpClient httpClient = new HttpClient();

        static void Main(string[] args)
        {
            // Sleep for some time to give IIS Express time to start the service
            Thread.Sleep(2000);
            ExecuteJsonBatchRequest().Wait();

            Console.WriteLine("Press any key to Exit ...");
            Console.ReadKey();
        }

        private static Task ExecuteJsonBatchRequest()
        {
            return Task.Run(async () =>
            {
                // Act
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri);
                request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                string requestJson = @"
{
    ""requests"": [{
            ""id"": ""-1"",
            ""method"": ""GET"",
            ""url"": """ + absoluteUri + @""",
            ""headers"": {
                ""OData-Version"": ""4.0"",
                ""Content-Type"": ""application/json;odata.metadata=minimal"",
                ""Accept"": ""application/json;odata.metadata=minimal""
            }
        }, {
            ""id"": ""0"",
            ""atomicityGroup"": ""f7de7314-2f3d-4422-b840-ada6d6de0f18"",
            ""method"": ""PATCH"",
            ""url"": """ + absoluteUri + "(6)" + @""",
            ""headers"": {
                ""OData-Version"": ""4.0"",
                ""Content-Type"": ""application/json;odata.metadata=minimal"",
                ""Accept"": ""application/json;odata.metadata=minimal""
            },
            ""body"": {
                ""Name"":""PatchedByJsonBatch_0""
            }
        }, {
            ""id"": ""1"",
            ""atomicityGroup"": ""f7de7314-2f3d-4422-b840-ada6d6de0f18"",
            ""method"": ""POST"",
            ""url"": """ + absoluteUri + @""",
            ""headers"": {
                ""OData-Version"": ""4.0"",
                ""Content-Type"": ""application/json;odata.metadata=minimal"",
                ""Accept"": ""application/json;odata.metadata=minimal""
            },
            ""body"": {
                ""Id"":1001,
                ""Name"":""CreatedByJsonBatch_11""
            }
        }, {
            ""id"": ""2"",
            ""atomicityGroup"": ""f7de7314-2f3d-4422-b840-ada6d6de0f18"",
            ""method"": ""POST"",
            ""url"": """ + absoluteUri + @""",
            ""headers"": {
                ""OData-Version"": ""4.0"",
                ""Content-Type"": ""application/json;odata.metadata=minimal"",
                ""Accept"": ""application/json;odata.metadata=minimal""
            },
            ""body"": {
                ""Id"":1002,
                ""Name"":""CreatedByJsonBatch_12""
            }
        }, {
            ""id"": ""3"",
            ""method"": ""POST"",
            ""url"": """ + absoluteUri + @""",
            ""headers"": {
                ""OData-Version"": ""4.0"",
                ""Content-Type"": ""application/json;odata.metadata=minimal"",
                ""Accept"": ""application/json;odata.metadata=minimal""
            },
            ""body"": {
                ""Id"": 1003,
                ""Name"":""CreatedByJsonBatch_3""
            }
        }
    ]
}";
                HttpContent content = new StringContent(requestJson);

                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;

                Console.WriteLine("Sending batch request...\r\n {0}", requestJson);
                HttpResponseMessage response = await httpClient.SendAsync(request);

                var stream = await response.Content.ReadAsStreamAsync();

                Console.WriteLine("Batch response code: {0}", response.StatusCode);
                Console.WriteLine("Batch response format: {0}", response.Content.Headers.ContentType.MediaType.ToString());

                IODataResponseMessage odataResponseMessage = new ODataMessageWrapper(stream, response.Content.Headers);
                int subResponseCount = 0;
                using (var messageReader = new ODataMessageReader(odataResponseMessage, new ODataMessageReaderSettings(), EdmModelHelper.GetModel()))
                {
                    var batchReader = messageReader.CreateODataBatchReader();

                    while (batchReader.Read())
                    {
                        switch (batchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationMessage = batchReader.CreateOperationResponseMessage();
                                subResponseCount++;

                                Console.WriteLine("\tOperation: ContentId: {0}, StatusCode: {1}, GroupId: {2}",
                                    operationMessage.ContentId, operationMessage.StatusCode, operationMessage.GroupId ?? "null");

                                break;
                        }
                    }
                }
                Console.WriteLine("\r\nTotal requests count: {0}", subResponseCount);
            });
        }
    }
}