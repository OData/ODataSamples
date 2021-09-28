using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.OData;
using Microsoft.OData.Edm;

namespace DynamicEdmModelCreation
{
    internal class ODataStreamSerializer : ODataEdmTypeSerializer
    {
        public ODataStreamSerializer(ODataSerializerProvider serializerProvider) : base(ODataPayloadKind.Property, serializerProvider)
        {
        }

        public override ODataValue CreateODataValue(object graph, IEdmTypeReference expectedType, ODataSerializerContext writeContext)
        {
            return graph as ODataStreamReferenceValue;
        }
    }
}