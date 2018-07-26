// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Net.Http;

namespace DynamicEdmModelCreation
{
    public class HttpRequestMessageProvider : IHttpRequestMessageProvider
    {
        /// <inheritdoc />
        public HttpRequestMessage Request { get; set; }
    }
}
