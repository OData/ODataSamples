// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Routing;

namespace DynamicEdmModelCreation.Extensions
{
    public class CustomODataRoute : ODataRoute
    {
        public CustomODataRoute(IRouter target, string routeName, string routePrefix, ODataPathRouteConstraint routeConstraint, IInlineConstraintResolver resolver)
            : base(target, routeName, routePrefix, routeConstraint, resolver)
        {
        }
    }
}