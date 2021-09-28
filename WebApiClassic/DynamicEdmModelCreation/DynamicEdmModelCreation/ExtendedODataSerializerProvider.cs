using System;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.OData.Edm;

namespace DynamicEdmModelCreation
{
    public class ExtendedODataSerializerProvider : DefaultODataSerializerProvider
    {
        private readonly ODataStreamSerializer streamSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedODataDeserializerProvider"/> class.
        /// </summary>
        public ExtendedODataSerializerProvider(IServiceProvider rootContainer)
            : base(rootContainer)
        {
            this.streamSerializer = new ODataStreamSerializer(this);
        }

        /// <inheritdoc />
        public override ODataEdmTypeSerializer GetEdmTypeSerializer(IEdmTypeReference edmType)
        {
            return edmType.IsStream() ? this.streamSerializer : base.GetEdmTypeSerializer(edmType);
        }
    }
}