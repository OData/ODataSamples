﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service.Providers
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;

    /// <summary>
    /// Implementation of <see cref="IEdmStructuralProperty"/> based on a <see cref="ResourceProperty"/>.
    /// </summary>
    internal class MetadataProviderEdmStructuralProperty : EdmStructuralProperty, IResourcePropertyBasedEdmProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataProviderEdmStructuralProperty"/> class.
        /// </summary>
        /// <param name="declaringType">The type that declares this property.</param>
        /// <param name="resourceProperty">The resource-property this edm property is based on.</param>
        /// <param name="type">The type of the property.</param>
        /// <param name="defaultValue">The default value of this property.</param>
        /// <param name="concurrencyMode">The concurrency mode of this property.</param>
        public MetadataProviderEdmStructuralProperty(
            IEdmStructuredType declaringType,
            ResourceProperty resourceProperty,
            IEdmTypeReference type, 
            string defaultValue,
            EdmConcurrencyMode concurrencyMode)
            : base(declaringType, resourceProperty.Name, type, defaultValue, concurrencyMode)
        {
            this.ResourceProperty = resourceProperty;
        }

        /// <summary>
        /// The <see cref="ResourceProperty"/> this edm property was created from.
        /// </summary>
        public ResourceProperty ResourceProperty { get; private set; }
    }
}
