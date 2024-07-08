// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service.Providers
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Extends <see cref="IEdmType"/> to expose the <see cref="ResourceType"/> that the type was based on.
    /// </summary>
    internal interface IResourceTypeBasedEdmType : IEdmType
    {
        /// <summary>
        /// The resource-type that this type was created from.
        /// </summary>
        ResourceType ResourceType { get; }
    }
}
