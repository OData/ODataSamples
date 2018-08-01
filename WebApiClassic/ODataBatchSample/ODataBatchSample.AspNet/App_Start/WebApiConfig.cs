// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using ODataBatchSample.Models;
using System.Web.Http;
using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;

using Microsoft.OData.Edm;

namespace ODataBatchSample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapODataServiceRoute("odata", "odata", GetModel(), new DefaultODataBatchHandler(GlobalConfiguration.DefaultServer));
        }

        private static IEdmModel GetModel()
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder();
            builder.ContainerName = "CustomersContext";
            builder.EntitySet<Customer>("Customers");
            return builder.GetEdmModel();
        }
    }
}
