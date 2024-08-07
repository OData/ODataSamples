﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service.Serializers
{
    #region Namespaces

    using System;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Atom;
    using Microsoft.OData.Service.Providers;
    #endregion Namespaces

    /// <summary>
    /// Serializer for writing service document format.
    /// </summary>
    internal sealed class ServiceDocumentSerializer
    {
        /// <summary>
        /// ODataMessageWriter instance which needs to be used for writing out the response payload.
        /// </summary>
        private readonly ODataMessageWriter writer;

        /// <summary>
        /// Creates a new instance of ServiceDocumentSerializer.
        /// </summary>
        /// <param name="writer">ODataMessageWriter instance to be used for writing out the response payload.</param>
        internal ServiceDocumentSerializer(ODataMessageWriter writer)
        {
            Debug.Assert(writer != null, "writer != null");
            this.writer = writer;
        }

        /// <summary>Writes the Service Document to the output stream.</summary>
        /// <param name="provider">DataServiceProviderWrapper instance.</param>
        internal void WriteServiceDocument(DataServiceProviderWrapper provider)
        {
            ODataServiceDocument serviceDocument = new ODataServiceDocument();
            serviceDocument.EntitySets = provider.GetResourceSets().Select(rs =>
            {
                ODataEntitySetInfo entitySetInfo = new ODataEntitySetInfo()
                {
                    Url = new Uri(rs.Name, UriKind.RelativeOrAbsolute),
                    Name = rs.Name
                };

                entitySetInfo.SetAnnotation<AtomResourceCollectionMetadata>(new AtomResourceCollectionMetadata()
                {
                    Title = new AtomTextConstruct { Text = rs.Name }
                });

                return entitySetInfo;
            });

            this.writer.WriteServiceDocument(serviceDocument);
        }
    }
}
