namespace Microsoft.Test.OData.Services.ODataWCFService.DataSource
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public interface IODataStreamProvider
    {
        Stream GetStream(object entity);

        void CreateStream(object entity, Stream stream, string contentType);

        void UpdateStream(object entity, Stream stream, string contentType);

        void DeleteStream(object entity);

        string GetETag(object entity);

        string GetContentType(object entity);
    }
}
