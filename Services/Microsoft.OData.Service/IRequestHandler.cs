// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service
{
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Web;    
    
    /// <summary>
    /// This interface declares the service contract for a DataWeb
    /// service.
    /// </summary>
    [ServiceContract]
    public interface IRequestHandler
    {
        /// <summary>Provides an entry point for the request. </summary>
        /// <returns>The resulting message for the supplied request.</returns>
        /// <param name="messageBody">The <see cref="T:System.IO.Stream" /> object that contains the request.</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "*", Method = "*")]
        Message ProcessRequestForMessage(Stream messageBody);
    }
}
