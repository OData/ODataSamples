// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using CustomODataFormatter.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.OData;
using Microsoft.OData.Edm;

namespace CustomODataFormatter.Extensions
{
    // A custom serializer provider to inject the AnnotatingEntitySerializer.
    public class CustomODataSerializerProvider : DefaultODataSerializerProvider
    {
        private AnnotatingEntitySerializer _annotatingEntitySerializer;

        public CustomODataSerializerProvider(IServiceProvider rootContainer)
            : base(rootContainer)
        {
            _annotatingEntitySerializer = new AnnotatingEntitySerializer(this);
        }

        public override ODataEdmTypeSerializer GetEdmTypeSerializer(IEdmTypeReference edmType)
        {
            if (edmType.IsEntity())
            {
                return _annotatingEntitySerializer;
            }

            return base.GetEdmTypeSerializer(edmType);
        }
    }


    // A custom entity serializer that adds the score annotation to document entries.
    public class AnnotatingEntitySerializer : ODataResourceSerializer
    {
        public AnnotatingEntitySerializer(ODataSerializerProvider serializerProvider)
            : base(serializerProvider)
        {
        }

        public override ODataResource CreateResource(SelectExpandNode selectExpandNode, ResourceContext resourceContext)
        {
            ODataResource entry = base.CreateResource(selectExpandNode, resourceContext);

            Document document = resourceContext.ResourceInstance as Document;
            if (entry != null && document != null)
            {
                // annotate the document with the score.
                entry.InstanceAnnotations.Add(new ODataInstanceAnnotation("org.northwind.search.score", new ODataPrimitiveValue(document.Score)));
            }

            return entry;
        }
    }
}
