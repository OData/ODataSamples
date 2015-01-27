// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.Test.OData.Services.ODataWCFService.Handlers
{
    using System.Net;
    using Microsoft.OData.Core;

    /// <summary>
    /// Class for handling $metadata requests.
    /// </summary>
    public class MetadataDocumentHandler : RequestHandler
    {
        public MetadataDocumentHandler(RequestHandler other)
            : base(other, HttpMethod.GET)
        {
        }

        public override void Process(IODataRequestMessage requestMessage, IODataResponseMessage responseMessage)
        {
            responseMessage.SetStatusCode(HttpStatusCode.OK);
            using (var writer = this.CreateMessageWriter(responseMessage))
            {
                writer.WriteMetadataDocument();
            }
        }
    }
}