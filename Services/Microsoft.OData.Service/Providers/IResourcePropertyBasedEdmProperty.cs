// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service.Providers
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Extends <see cref="IEdmProperty"/> to expose the <see cref="ResourceProperty"/> that the property was based on.
    /// </summary>
    internal interface IResourcePropertyBasedEdmProperty : IEdmProperty
    {
        /// <summary>
        /// The <see cref="ResourceProperty"/> this edm property was created from.
        /// </summary>
        ResourceProperty ResourceProperty { get; }
    }
}
