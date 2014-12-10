using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;

namespace ODataSamples.CustomFormatService.Formaters.CSV
{
    internal class CsvWriter : ODataWriter
    {
    private CsvOutputContext context;
    private List<string> headers;

    public CsvWriter(CsvOutputContext context, IEdmEntityType entityType)
    {
        this.context = context;
        this.WriteHeader(entityType);
    }

    public override void Flush()
    {
        this.context.Flush();
    }

    public override void WriteEnd()
    {
    }

    public override void WriteStart(ODataFeed feed)
    {
    }

    public override void WriteStart(ODataEntry entry)
    {
        this.WriteEntry(entry);
    }

    private void WriteHeader(IEdmEntityType entityType)
    {
        this.headers = entityType.Properties().Where(p => p.Type.IsPrimitive()).Select(p => p.Name).ToList();
        foreach (var header in this.headers)
        {
            this.context.Writer.Write("{0},", header);
        }

        this.context.Writer.WriteLine();
    }


    private void WriteEntry(ODataEntry entry)
    {
        foreach (var header in this.headers)
        {
            var property = entry.Properties.SingleOrDefault(p => p.Name == header);
            if (property != null)
            {
                this.context.Writer.Write(property.Value);
            }

            this.context.Writer.Write(",");
        }

        this.context.Writer.WriteLine();
    }

        #region not implemented
        public override void WriteEntityReferenceLink(ODataEntityReferenceLink entityReferenceLink)
        {
            throw new NotImplementedException();
        }

        public override void WriteStart(ODataNavigationLink navigationLink)
        {
            throw new NotImplementedException();
        }
        public override System.Threading.Tasks.Task FlushAsync()
        {
            throw new NotImplementedException();
        }
        public override System.Threading.Tasks.Task WriteEndAsync()
        {
            throw new NotImplementedException();
        }
        public override System.Threading.Tasks.Task WriteEntityReferenceLinkAsync(ODataEntityReferenceLink entityReferenceLink)
        {
            throw new NotImplementedException();
        }
        public override System.Threading.Tasks.Task WriteStartAsync(ODataNavigationLink navigationLink)
        {
            throw new NotImplementedException();
        }
        public override System.Threading.Tasks.Task WriteStartAsync(ODataEntry entry)
        {
            throw new NotImplementedException();
        }

        public override System.Threading.Tasks.Task WriteStartAsync(ODataFeed feed)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}