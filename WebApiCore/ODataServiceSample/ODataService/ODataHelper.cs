// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.OData.UriParser;

namespace ODataService
{
    /// <summary>
    /// Helper class to facilitate building an odata service.
    /// </summary>
    public static class ODataHelper
    {
        /// <summary>
        /// Helper method to get the key value from a uri.
        /// Usually used by $ref action to extract the key value from the url in body.
        /// </summary>
        /// <typeparam name="TKey">The type of the key</typeparam>
        /// <param name="request">The request instance in current context</param>
        /// <param name="uri">OData uri that contains the key value</param>
        /// <returns>The key value</returns>
        public static TKey GetKeyValue<TKey>(this HttpRequest request, Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            //get the odata path Ex: ~/entityset/key/navigation/$ref
            var serviceRoot = GetServiceRoot(request);
            var provider = request.GetRequestContainer();
            var odataPath = request.GetPathHandler().Parse(serviceRoot, uri.LocalPath, provider);

            var keySegment = odataPath.Segments.OfType<KeySegment>().FirstOrDefault();
            if (keySegment == null)
            {
                throw new InvalidOperationException("The link does not contain a key.");
            }

            // var value = ODataUriUtils.ConvertFromUriLiteral(keySegment.Value, ODataVersion.V4);
            return (TKey)keySegment.Keys.First().Value;
        }

        private static string GetServiceRoot(HttpRequest request)
        {
            var urlHelper = request.GetUrlHelper();
            return urlHelper.CreateODataLink(request.ODataFeature().RouteName, request.GetPathHandler(), new List<ODataPathSegment>());
        }
    }
}