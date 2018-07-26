// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.OData.UriParser;
using ODataPath = Microsoft.AspNet.OData.Routing.ODataPath;

namespace DynamicEdmModelCreation
{
    public class MatchAllRoutingConvention : IODataRoutingConvention
    {
        public string SelectAction(
            ODataPath odataPath,
            HttpControllerContext controllerContext,
            ILookup<string, HttpActionDescriptor> actionMap)
        {
            if (odataPath.PathTemplate == "~/entityset/key/navigation")
            {
                if (controllerContext.Request.Method == HttpMethod.Get)
                {
                    NavigationPropertySegment navigationPathSegment = (NavigationPropertySegment)odataPath.Segments.Last();

                    controllerContext.RouteData.Values["navigation"] = navigationPathSegment.NavigationProperty.Name;

                    KeySegment keyValueSegment = (KeySegment)odataPath.Segments[1];
                    controllerContext.RouteData.Values[ODataRouteConstants.Key] = keyValueSegment.Keys.First().Value;

                    return "GetNavigation";
                }
            }

            return null;
        }

        public string SelectController(ODataPath odataPath, HttpRequestMessage request)
        {
            return (odataPath.Segments.FirstOrDefault() is EntitySetSegment) ? "HandleAll" : null;
        }
    }
}