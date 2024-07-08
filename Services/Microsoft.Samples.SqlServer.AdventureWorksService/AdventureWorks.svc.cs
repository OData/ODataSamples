// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Samples.SqlServer.AdventureWorksService
{
    using System;
    using System.Configuration;
    using System.Data.Services;
    using System.Data.Services.Common;
    using System.Data.Services.Providers;
    using System.ServiceModel;
    using System.Web;

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class AdventureWorks : EntityFrameworkDataService<AdventureWorksEntities>
    {
        /// <summary>
        /// Initialize service for the first time
        /// </summary>
        /// <param name="config">Data service configuration</param>
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.None);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
            config.SetEntitySetPageSize("*", 500);
            config.SetEntitySetPageSize("ProductCatalog", 500);
            config.SetEntitySetPageSize("WorkOrderRouting", 1000);
            config.SetEntitySetPageSize("TerritorySalesDrilldown", 500);
            config.SetEntitySetPageSize("CompanySales", 500);
            config.SetEntitySetPageSize("ManufacturingInstructions", 500);
            config.UseVerboseErrors = true;
        }

        /// <summary>
        /// OnStartProcessingRequest is overriden to get Caching support
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStartProcessingRequest(ProcessRequestArgs args)
        {
            base.OnStartProcessingRequest(args);

            int cacheTimeout;
            if (Int32.TryParse(ConfigurationManager.AppSettings["cacheTimeout"], out cacheTimeout) && cacheTimeout > 0)
            {
                HttpContext context = HttpContext.Current;
                HttpCachePolicy c = HttpContext.Current.Response.Cache;
                c.SetCacheability(HttpCacheability.ServerAndPrivate);
                c.SetExpires(HttpContext.Current.Timestamp.AddSeconds(cacheTimeout));
                c.VaryByHeaders["Accept"] = true;
                c.VaryByHeaders["Accept-Charset"] = true;
                c.VaryByHeaders["Accept-Encoding"] = true;
                c.VaryByParams["*"] = true;
            }
        }
    }
}
