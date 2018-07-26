// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using DynamicEdmModelCreation.DataSource;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using Microsoft.OData.Edm;
using ServiceLifetime = Microsoft.OData.ServiceLifetime;

namespace DynamicEdmModelCreation.Extensions
{
    public static class RouteBuilderExtension
    {
        public static ODataRoute CustomMapODataServiceRoute(this IRouteBuilder routeBuilder, string routeName, string routePrefix)
        {
            ODataRoute route = routeBuilder.MapODataServiceRoute(routeName, routePrefix, builder =>
            {
                // Get the model from the datasource of the current request: model-per-pequest.
                builder.AddService(ServiceLifetime.Scoped, sp =>
                {
                    var serviceScope = sp.GetRequiredService<HttpRequestScope>();

                    // serviceScope.
                    string sourceString = serviceScope.HttpRequest.GetDataSource();
                    IEdmModel model = DataSourceProvider.GetEdmModel(sourceString);

                    return model;
                });

                // The routing conventions are registered as singleton.
                builder.AddService(ServiceLifetime.Singleton, sp =>
                {
                    IList<IODataRoutingConvention> routingConventions = ODataRoutingConventions.CreateDefault();
                    routingConventions.Insert(0, new MatchAllRoutingConvention());
                    return routingConventions.ToList().AsEnumerable();
                });
            });

           // route.Constraints.
            IRouter customRouter = routeBuilder.ServiceProvider.GetService<IRouter>();

            // Get constraint resolver.
            IInlineConstraintResolver inlineConstraintResolver = routeBuilder.ServiceProvider.GetRequiredService<IInlineConstraintResolver>();

            CustomODataRoute odataRoute = new CustomODataRoute(customRouter != null ? customRouter : routeBuilder.DefaultHandler,
                routeName,
                routePrefix,
                new CustomODataPathRouteConstraint(routeName),
                inlineConstraintResolver);

            routeBuilder.Routes.Remove(route);
            routeBuilder.Routes.Add(odataRoute);

            return odataRoute;
        }
    }
}
