// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using DynamicEdmModelCreation.Extensions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace DynamicEdmModelCreation
{
    public class CustomODataPathRouteConstraint : ODataPathRouteConstraint
    {
        public CustomODataPathRouteConstraint(string routeName)
            : base(routeName)
        {
        }

        public override bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (routeDirection == RouteDirection.IncomingRequest)
            {
                object dataSource;
                if (values.TryGetValue("dataSource", out dataSource))
                {
                    httpContext.Request.SetDataSource((string)dataSource);
                }
            }

            return base.Match(httpContext, route, routeKey, values, routeDirection);
        }
    }
}