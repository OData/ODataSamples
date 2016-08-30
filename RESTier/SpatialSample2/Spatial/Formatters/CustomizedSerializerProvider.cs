// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Web.OData.Formatter.Serialization;
using Microsoft.OData.Edm;
using Microsoft.Restier.Publishers.OData.Formatter;

namespace Microsoft.OData.Service.Sample.Spatial2.Formatters
{
    /// <summary>
    /// Used to create customized primitive type serializer
    /// Cannot just add a RestierPayloadValueConverter, as this is called just before object been write in method WriteObject.
    /// The converted must happen before create primitive type Edm type property in method CreateODataPrimitiveValue
    /// </summary>
    public class CustomizedSerializerProvider : DefaultRestierSerializerProvider
    {
        private RestierPrimitiveSerializer primitiveSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRestierSerializerProvider" /> class.
        /// </summary>
        public CustomizedSerializerProvider(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.primitiveSerializer = new CustomizedPrimitiveSerializer();
        }

        /// <summary>
        /// Gets the serializer for the given result type.
        /// </summary>
        /// <param name="type">The type of result to serialize.</param>
        /// <param name="request">The HTTP request.</param>
        /// <returns>The serializer instance.</returns>
        public override ODataSerializer GetODataPayloadSerializer(
            Type type,
            HttpRequestMessage request)
        {
            ODataSerializer serializer = null;
            if (type.Name == "PrimitiveResult")
            {
                serializer = this.primitiveSerializer;
            }
            else
            {
                serializer = base.GetODataPayloadSerializer(type, request);
            }

            return serializer;
        }

        /// <summary>
        /// Gets the serializer for the given EDM type reference.
        /// </summary>
        /// <param name="edmType">The EDM type reference involved in the serializer.</param>
        /// <returns>The serializer instance.</returns>
        public override ODataEdmTypeSerializer GetEdmTypeSerializer(IEdmTypeReference edmType)
        {
            if (edmType.IsPrimitive())
            {
                return this.primitiveSerializer;
            }

            return base.GetEdmTypeSerializer(edmType);
        }
    }
}