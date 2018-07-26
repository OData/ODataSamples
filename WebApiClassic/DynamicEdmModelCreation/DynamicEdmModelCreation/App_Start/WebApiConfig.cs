// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Web.Http;
using Microsoft.AspNet.OData.Extensions;

namespace DynamicEdmModelCreation
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.CustomMapODataServiceRoute("odata", "odata");

            config.AddODataQueryFilter();
        }
    }
}
