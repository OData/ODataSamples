using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;

namespace ODataSamples.CustomFormatService.Formaters.CSV
{
internal sealed class CsvOutputContext : ODataOutputContext
{
    private Stream stream;

    public CsvOutputContext(
        ODataFormat format,
        ODataMessageWriterSettings settings,
        ODataMessageInfo messageInfo,
        bool synchronous)
        : base(format, settings, messageInfo.IsResponse, synchronous, messageInfo.Model, messageInfo.UrlResolver)
    {
        this.stream = messageInfo.GetMessageStream();
        this.Writer = new StreamWriter(this.stream);
    }

    public TextWriter Writer { get; private set; }

    public override ODataWriter CreateODataEntryWriter(IEdmNavigationSource navigationSource, IEdmEntityType entityType)
    {
        return new CsvWriter(this, entityType);
    }

    public override ODataWriter CreateODataFeedWriter(IEdmEntitySetBase entitySet, IEdmEntityType entityType)
    {
        return new CsvWriter(this, entityType);
    }

    public void Flush()
    {
        this.stream.Flush();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            try
            {
                if (this.Writer != null)
                {
                    this.Writer.Dispose();
                }

                if (this.stream != null)
                {
                    this.stream.Dispose();
                }
            }
            finally
            {
                this.Writer = null;
                this.stream = null;
            }
        }

        base.Dispose(disposing);
    }
}
}