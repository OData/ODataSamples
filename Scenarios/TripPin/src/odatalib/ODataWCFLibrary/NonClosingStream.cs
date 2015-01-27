//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//
// Owner:        ashfarn
// Backup Owner: zheyu
//

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System.IO;

    /// <summary>
    /// Internal memory stream that does nothing when Close is called on the stream
    /// This is required for writing out errors as the WriteODataError method on the writer closes the stream
    /// Since we return streams from our wcf methods, this causes us to return a closed stream, throwing an error during serialization
    /// </summary>
    public class NonClosingStream : MemoryStream
    {
        public override void Close()
        {
            this.Seek(0, SeekOrigin.Begin);
        }

        protected override void Dispose(bool disposing)
        {
            this.Close();
        }
    }
}