// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service.Providers
{
    /// <summary>
    /// Internal enum for indicating whether the <see cref="MetadataProviderEdmModel"/> is currently being used for reading/writing payloads or parsing URIs.
    /// </summary>
    internal enum MetadataProviderEdmModelMode
    {
        /// <summary>
        /// Indicates that the model is being used for reading/writing payloads.
        /// </summary>
        Serialization,

        /// <summary>
        /// Indicates that the model is being used for parsing $select/$expand.
        /// </summary>
        SelectAndExpandParsing,

        /// <summary>
        /// Indicates that the model is being used for URI path parsing.
        /// </summary>
        UriPathParsing,
    }
}
