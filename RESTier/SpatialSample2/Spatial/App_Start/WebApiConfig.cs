// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Web.Http;
using Microsoft.OData.Service.Sample.Spatial2.Api;
using Microsoft.Restier.Publishers.OData;
using Microsoft.Restier.Publishers.OData.Batch;

namespace Microsoft.OData.Service.Sample.Spatial2
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            RegisterSpatial(config, GlobalConfiguration.DefaultServer);
        }

        public static async void RegisterSpatial(
            HttpConfiguration config, HttpServer server)
        {
            await config.MapRestierRoute<SpatialApi>(
                "SpatialApi", "api/spatial2",
                new RestierBatchHandler(server));
        }
    }
}
