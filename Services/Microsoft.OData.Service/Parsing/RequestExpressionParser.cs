﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service.Parsing
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Service.Providers;

    #endregion Namespaces

    /// <summary>Use this class to parse an expression in the Astoria URI format.</summary>
    [DebuggerDisplay("ExpressionParser ({lexer.text})")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Won't Fix")]
    internal class RequestExpressionParser
    {
        #region Fields

        /// <summary>Provider of data and metadata.</summary>
        private readonly DataServiceProviderWrapper provider;

        /// <summary>The service's metadata-provider-based edm-model.</summary>
        private readonly MetadataProviderEdmModel model;

        /// <summary>ODataUriParser instance </summary>
        private readonly ODataUriParser odataUriParser;
        #endregion Fields

        #region Constructors

        /// <summary>Initializes a new <see cref="RequestExpressionParser"/>.</summary>
        /// <param name="service">Service with data and configuration.</param>
        /// <param name="requestDescription">RequestDescription instance containing information about the current request being parsed.</param>
        internal RequestExpressionParser(IDataService service, RequestDescription requestDescription)
        {
            Debug.Assert(service != null, "service != null");
            Debug.Assert(requestDescription != null, "requestDescription != null");

            this.provider = service.Provider;

            this.model = this.provider.GetMetadataProviderEdmModel();

            this.odataUriParser = RequestUriProcessor.CreateUriParserWithBatchReferenceCallback(service, service.OperationContext.AbsoluteRequestUri);
        }

        #endregion Constructors

        /// <summary>Parses the text expression for $filter.</summary>
        /// <returns>The parsed filter clause.</returns>
        internal FilterClause ParseFilter()
        {
            try
            {
                return odataUriParser.ParseFilter();
            }
            catch (ODataException ex)
            {
                throw new DataServiceException(400, null, ex.Message, null, ex);
            }
        }

        /// <summary>Parses the text expression for ordering.</summary>
        /// <returns>An orderby clause.</returns>
        internal OrderByClause ParseOrderBy()
        {
            try
            {
                return odataUriParser.ParseOrderBy();
            }
            catch (ODataException ex)
            {
                throw new DataServiceException(400, null, ex.Message, null, ex);
            }
        }
    }
}
