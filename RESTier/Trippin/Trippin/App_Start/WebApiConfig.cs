// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Web.Http;
using Microsoft.OData.Service.Sample.Trippin.Api;
using Microsoft.AspNet.OData.Query;
using Microsoft.Restier.EntityFramework;
using Microsoft.OData.Service.Sample.Trippin.Models;
using Microsoft.Restier.Core.Submit;
using Microsoft.OData.Service.Sample.Trippin.Submit;
using Microsoft.Restier.Core.Model;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Restier.Core;

namespace Microsoft.OData.Service.Sample.Trippin
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Filter().Expand().Select().OrderBy().MaxTop(null).Count();

            config.UseRestier<TrippinApi>((services) =>
                {
                    services.AddEF6ProviderServices<TrippinModel>();
                    services.AddSingleton<ODataPayloadValueConverter, CustomizedPayloadValueConverter>();
                    services.AddSingleton<ODataValidationSettings>(
                        new ODataValidationSettings
                        {
                            MaxAnyAllExpressionDepth = 3,
                            MaxExpansionDepth = 3
                        }
                    );

                    services.AddSingleton<IChangeSetItemFilter, CustomizedSubmitProcessor>();
                    services.AddChainedService<IModelBuilder>((sp, next) => new TrippinApi.TrippinModelExtender(next));
                }
            );

            config.MapRestier<TrippinApi>(
                    "TrippinApi",
                    "",
                    true);
        }
    }
}