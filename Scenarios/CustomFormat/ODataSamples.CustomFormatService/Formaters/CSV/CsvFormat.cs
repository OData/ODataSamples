using Microsoft.OData.Core;

namespace ODataSamples.CustomFormatService.Formaters.CSV
{
public class CsvFormat : ODataFormat
{
    public override ODataOutputContext CreateOutputContext(ODataMessageInfo messageInfo, ODataMessageWriterSettings messageWriterSettings)
    {
        return new CsvOutputContext(this, messageWriterSettings, messageInfo, synchronous: true);
    }


        public override ODataInputContext CreateInputContext(ODataMessageInfo messageInfo, ODataMessageReaderSettings messageReaderSettings)
        {
            throw new System.NotImplementedException();
        }

        public override System.Threading.Tasks.Task<ODataInputContext> CreateInputContextAsync(ODataMessageInfo messageInfo, ODataMessageReaderSettings messageReaderSettings)
        {
            throw new System.NotImplementedException();
        }


        public override System.Threading.Tasks.Task<ODataOutputContext> CreateOutputContextAsync(ODataMessageInfo messageInfo, ODataMessageWriterSettings messageWriterSettings)
        {
            throw new System.NotImplementedException();
        }

        public override System.Collections.Generic.IEnumerable<ODataPayloadKind> DetectPayloadKind(ODataMessageInfo messageInfo, ODataMessageReaderSettings settings)
        {
            throw new System.NotImplementedException();
        }

        public override System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<ODataPayloadKind>> DetectPayloadKindAsync(ODataMessageInfo messageInfo, ODataMessageReaderSettings settings)
        {
            throw new System.NotImplementedException();
        }
    }
}