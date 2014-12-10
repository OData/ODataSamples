using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Core;

namespace ODataSamples.CustomFormatService.Formaters.CSV
{
public class CsvMediaTypeResolver : ODataMediaTypeResolver
{
    private static readonly CsvMediaTypeResolver instance = new CsvMediaTypeResolver();
    private readonly ODataMediaTypeFormat[] mediaTypeFormats =
    {
        new ODataMediaTypeFormat(new ODataMediaType("text", "csv"), new CsvFormat())
    };

    private CsvMediaTypeResolver() { }

    public static CsvMediaTypeResolver Instance { get { return instance; } }

    public override IEnumerable<ODataMediaTypeFormat> GetMediaTypeFormats(ODataPayloadKind payloadKind)
    {
        if (payloadKind == ODataPayloadKind.Entry || payloadKind== ODataPayloadKind.Feed)
        {
            return mediaTypeFormats.Concat(base.GetMediaTypeFormats(payloadKind));
        }

        return base.GetMediaTypeFormats(payloadKind);
    }
}
}