﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System.Collections;
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
    /// An <see cref="IEdmFunction"/> implementation backed by an IDSMP metadata provider.
    /// </summary>
    internal sealed class MetadataProviderEdmFunction : MetadataProviderEdmOperation, IEdmFunction
    {
        /// <summary>Default value of the IsComposable property.</summary>
        private const bool DefaultIsComposable = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataProviderEdmFunctionImport"/> class.
        /// </summary>
        /// <param name="model">The model this instance belongs to.</param>
        /// <param name="operation">The resource operation underlying this function import.</param>
        /// <param name="namespaceName">The namespace of the operation.</param>
        /// <remarks>
        /// This constructor assumes that the entity set for this service operation has already be created.
        /// </remarks>
        internal MetadataProviderEdmFunction(MetadataProviderEdmModel model, OperationWrapper operation, string namespaceName)
            : base(model, operation, namespaceName)
        {
            this.IsComposable = DefaultIsComposable;
            
            // By default everything is composable except functions that return IEnumerable
            if (operation.ReturnInstanceType != null && !(typeof(IEnumerable).IsAssignableFrom(operation.ReturnInstanceType) && !typeof(IQueryable).IsAssignableFrom(operation.ReturnInstanceType)))
            {
                this.IsComposable = true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is composable.
        /// </summary>
        public bool IsComposable { get; private set; }

        /// <summary>
        /// Gets the kind of this schema element.
        /// </summary>
        public override EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.Function; }
        }
    }
}
