// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service.Providers
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Extends <see cref="IEdmEntitySet"/> to expose the <see cref="ResourceSetWrapper"/> that the entity set was based on.
    /// </summary>
    internal interface IResourceSetBasedEdmEntitySet : IEdmEntitySet
    {
        /// <summary>
        /// The resource-set wrapper that this entity-set was created from.
        /// </summary>
        ResourceSetWrapper ResourceSet { get; }
    }
}
