﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Expressions;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm.Library.Expressions;


    #endregion Namespaces

    /// <summary>
    /// An <see cref="IEdmFunctionImport"/> implementation backed by an IDSMP metadata provider.
    /// </summary>
    internal sealed class MetadataProviderEdmFunctionImport : MetadataProviderEdmOperationImport, IEdmFunctionImport
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataProviderEdmFunctionImport"/> class.
        /// </summary>
        /// <param name="model">The model this instance belongs to.</param>
        /// <param name="container">The container this instance belongs to.</param>
        /// <param name="function">The function that is being imported.</param>
        /// <remarks>
        /// This constructor assumes that the entity set for this service operation has already be created.
        /// </remarks>
        internal MetadataProviderEdmFunctionImport(
            MetadataProviderEdmModel model, 
            MetadataProviderEdmEntityContainer container, 
            MetadataProviderEdmFunction function)
            : base(model, container, function)
        {
            this.Function = function;
        }

        /// <summary>
        /// Gets the operation.
        /// </summary>
        public IEdmFunction Function { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [include in service document].
        /// </summary>
        public bool IncludeInServiceDocument
        {
            // Hardcoding this to false otherwise model validation will fail now.
            get { return false; }
        }

        /// <summary>
        /// The container element kind; EdmContainerElementKind.ActionImport for operation imports.
        /// </summary>
        public override EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.FunctionImport; }
        }
    }
}
