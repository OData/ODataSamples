// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Http;

namespace DynamicEdmModelCreation.Extensions
{
    public static class HttpRequestExtensions
    {
        private const string DataSourceKey = "DynamicEdmModelCreatingDataSource_BF938AEC-355E-4673-8DB7-EE15DBA9B507";

        public static void SetDataSource(this HttpRequest request, string dataSource)
        {
            request.ODataFeature().RoutingConventionsStore.Add(DataSourceKey, dataSource);
        }

        public static string GetDataSource(this HttpRequest request)
        {
            return (string)request.ODataFeature().RoutingConventionsStore[DataSourceKey];
        }
    }
}
