// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Net.Http;

namespace DynamicEdmModelCreation
{
    // based on github https://github.com/OData/ODataSamples/pull/67
    public interface IHttpRequestMessageProvider
    {
        HttpRequestMessage Request { get; set; }
    }
}
