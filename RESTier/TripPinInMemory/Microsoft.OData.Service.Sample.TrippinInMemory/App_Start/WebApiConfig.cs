// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Service.Sample.TrippinInMemory.Api;
using Trippin;
using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Model;
using Microsoft.Restier.Core.Submit;
using Microsoft.Restier.Providers.InMemory.DataStoreManager;
using Microsoft.Restier.Providers.InMemory.Submit;
using System.Configuration;
using System.Web.Http.Routing;
using System.Net.Http;
using System.Collections.Generic;

namespace Microsoft.OData.Service.Sample.TrippinInMemory
{
    public static class WebApiConfig
    {
        private const string routeName = "TrippinApi";

        public static void Register(HttpConfiguration config)
        {
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;
            config.MessageHandlers.Add(new ETagMessageHandler());
            config.SetUrlKeyDelimiter(ODataUrlKeyDelimiter.Slash);
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));
            config.Routes.MapHttpRoute("Options", "{*OPTIONS}", new { controller = "CORS", action = "Options" }, new { Options = new myHttpRouteConstraint()});
            config.UseRestier<TrippinApi>((services) =>
            {
                Func<IServiceProvider, IDataStoreManager<string, TripPinDataSource>> defaultDataStoreManager =
                sp => new DefaultDataStoreManager<string, TripPinDataSource>()
                {
                    MaxDataStoreInstanceCapacity = 1000,
                    MaxDataStoreInstanceLifeTime = new TimeSpan(0, 30, 0)
                };

                Func<IServiceProvider, ODataValidationSettings> validationSettingFactory = sp => new ODataValidationSettings
                {
                    MaxAnyAllExpressionDepth = 4,
                    MaxExpansionDepth = 4
                };

                services.AddSingleton<ODataValidationSettings>(validationSettingFactory);
                services.AddChainedService<IModelBuilder>((sp, next) => new TrippinApi.ModelBuilder());
                services.AddChainedService<IChangeSetInitializer>((sp, next) => new ChangeSetInitializer<TripPinDataSource>());
                services.AddChainedService<ISubmitExecutor>((sp, next) => new SubmitExecutor());
                services.AddSingleton(defaultDataStoreManager);

                // Add custom TrippinBatchHandler
                ODataBatchHandler trippinBatchHandler = new TrippinBatchHandler(GlobalConfiguration.DefaultServer);
                trippinBatchHandler.ODataRouteName = routeName;
                services.AddSingleton(trippinBatchHandler);
            });

            RegisterTrippin(config, GlobalConfiguration.DefaultServer);
            
        }

        public static void RegisterTrippin(
            HttpConfiguration config, HttpServer server)
        {
            // enable query options for all properties
            config.Filter().Expand().Select().OrderBy().MaxTop(null).Count();
            config.SetTimeZoneInfo(TimeZoneInfo.Utc);
            config.MapRestier<TrippinApi>(
                routeName,
                "",
                false); // Custom TrippinBatchHandler registered in UseRestier 
        }
    }
    public class myHttpRouteConstraint : IHttpRouteConstraint
    {
        bool IHttpRouteConstraint.Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            return request.Method == HttpMethod.Options;
        }
    }
}