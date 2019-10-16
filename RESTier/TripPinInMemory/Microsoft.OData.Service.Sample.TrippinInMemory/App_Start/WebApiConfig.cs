// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.OData.Service.Sample.TrippinInMemory.Api;

namespace Microsoft.OData.Service.Sample.TrippinInMemory
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;
            config.MessageHandlers.Add(new ETagMessageHandler());
            config.SetUrlKeyDelimiter(ODataUrlKeyDelimiter.Slash);
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));
            RegisterTrippin(config, GlobalConfiguration.DefaultServer);
        }

        public static async void RegisterTrippin(
            HttpConfiguration config, HttpServer server)
        {
            // enable query options for all properties
            config.Filter().Expand().Select().OrderBy().MaxTop(null).Count();
            config.SetTimeZoneInfo(TimeZoneInfo.Utc);
            await config.MapRestierRoute<TrippinApi>(
                "TrippinApi",
                "",
                new TrippinBatchHandler(server));
        }
    }
}