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
    /// An <see cref="IEdmAction"/> implementation backed by an IDSMP metadata provider.
    /// </summary>
    internal sealed class MetadataProviderEdmAction : MetadataProviderEdmOperation, IEdmAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataProviderEdmActionImport"/> class.
        /// </summary>
        /// <param name="model">The model this instance belongs to.</param>
        /// <param name="operation">The resource operation underlying this action import.</param>
        /// <param name="namespaceName">The namespace of the EdmOperation.</param>
        /// <remarks>
        /// This constructor assumes that the entity set for this service operation has already be created.
        /// </remarks>
        internal MetadataProviderEdmAction(
            MetadataProviderEdmModel model, 
            OperationWrapper operation,
            string namespaceName)
            : base(model, operation, namespaceName)
        {
        }

        /// <summary>
        /// Gets the kind of this schema element.
        /// </summary>
        public override EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.Action; }
        }
    }
}
