// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.OData.UriParser;

namespace ODataSingletonSample
{
    public static class HelperFunction
    {
        public static TKey GetKeyValue<TKey>(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            // Calculate root Uri
            var rootPath = uri.AbsoluteUri.Substring(0, uri.AbsoluteUri.LastIndexOf('/') + 1);

            var odataUriParser = new ODataUriParser(SingletonEdmModel.GetEdmModel(), new Uri(rootPath), uri);
            var odataPath = odataUriParser.ParsePath();
            var keySegment = odataPath.LastSegment as KeySegment;
            if (keySegment == null)
            {
                throw new InvalidOperationException("The link does not contain a key.");
            }

            return (TKey)keySegment.Keys.First().Value;
        }
    }
}