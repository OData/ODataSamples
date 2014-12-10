using System.Collections.Generic;
using System.Web.OData.Formatter;
using System.Web.OData.Formatter.Deserialization;
using System.Web.OData.Formatter.Serialization;
using Microsoft.OData.Core;
using ODataSamples.CustomFormatService.Formaters.CSV;
using ODataSamples.CustomFormatService.Formaters.VCard;

namespace ODataSamples.CustomFormatService
{
    public class CustomFormatter : ODataMediaTypeFormatter
    {
        public CustomFormatter(ODataDeserializerProvider oDataDeserializerProvider,
            ODataSerializerProvider oDataSerializerProvider, params ODataPayloadKind[] kinds)
            : base(oDataDeserializerProvider, oDataSerializerProvider, kinds)
        {
            this.MessageWriterSettings.MediaTypeResolver = new MixResolver();
        }
    }

    public class MixResolver : ODataMediaTypeResolver
    {
        public override IEnumerable<ODataMediaTypeFormat> GetMediaTypeFormats(ODataPayloadKind payloadKind)
        {
            if (payloadKind == ODataPayloadKind.Property)
            {
                return VCardMediaTypeResolver.Instance.GetMediaTypeFormats(payloadKind);
            }
            else if (payloadKind == ODataPayloadKind.Entry || payloadKind == ODataPayloadKind.Feed)
            {
                return CsvMediaTypeResolver.Instance.GetMediaTypeFormats(payloadKind);
            }

            return base.GetMediaTypeFormats(payloadKind);
        }
    }
}