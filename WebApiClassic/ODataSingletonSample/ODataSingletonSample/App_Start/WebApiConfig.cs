// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Web.Http;
using Microsoft.AspNet.OData.Extensions;

namespace ODataSingletonSample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Select().Expand().Filter().OrderBy().MaxTop(100).Count();

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            config.MapODataServiceRoute("odata", "odata", SingletonEdmModel.GetEdmModel());
        }
    }
}
