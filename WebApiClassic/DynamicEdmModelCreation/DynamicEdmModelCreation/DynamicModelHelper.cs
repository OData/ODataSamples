// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DynamicEdmModelCreation.DataSource;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using Microsoft.OData.Edm;
using ServiceLifetime = Microsoft.OData.ServiceLifetime;

namespace DynamicEdmModelCreation
{
    public static class DynamicModelHelper
    {
        public static ODataRoute CustomMapODataServiceRoute(this HttpConfiguration configuration, string routeName, string routePrefix)
        {
            ODataRoute route = configuration.MapODataServiceRoute(routeName, routePrefix, builder =>
            {
                // Get the model from the datasource of the current request: model-per-pequest.
                builder.AddService(ServiceLifetime.Scoped, sp =>
                {
                    IHttpRequestMessageProvider requestMessageProvider = sp.GetRequiredService<IHttpRequestMessageProvider>();
                    string dataSource = requestMessageProvider.Request.Properties[Constants.ODataDataSource] as string;
                    IEdmModel model = DataSourceProvider.GetEdmModel(dataSource);
                    return model;
                });

                // Create a request provider for every request. This is a workaround for the missing HttpContext of a self-hosted webapi.
                builder.AddService<IHttpRequestMessageProvider>(ServiceLifetime.Scoped, sp => new HttpRequestMessageProvider());

                // The routing conventions are registered as singleton.
                builder.AddService(ServiceLifetime.Singleton, sp =>
                {
                    IList<IODataRoutingConvention> routingConventions = ODataRoutingConventions.CreateDefault();
                    routingConventions.Insert(0, new MatchAllRoutingConvention());
                    return routingConventions.ToList().AsEnumerable();
                });
            });

            CustomODataRoute odataRoute = new CustomODataRoute(route.RoutePrefix, new CustomODataPathRouteConstraint(routeName));
            configuration.Routes.Remove(routeName);
            configuration.Routes.Add(routeName, odataRoute);

            return odataRoute;
        }
    }
}