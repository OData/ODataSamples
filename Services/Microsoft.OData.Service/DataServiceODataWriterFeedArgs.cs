﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service
{
    using System.Collections;
    using System.Diagnostics;
    using Microsoft.OData.Core;

    /// <summary>
    /// Class that keeps track of the ODataFeed, collection instance and other information
    /// that we need to provide to the service author when they choose to provide their own
    /// instance of ODataWriter.
    /// </summary>
    public sealed class DataServiceODataWriterFeedArgs
    {
        /// <summary>
        /// Creates a new instance of DataServiceODataWriterFeedArgs
        /// </summary>
        /// <param name="feed">ODataFeed instance.</param>
        /// <param name="results">IEnumerable instance that is getting serialized.</param>
        /// <param name="operationContext">DataServiceOperationContext instance.</param>
        public DataServiceODataWriterFeedArgs(ODataFeed feed, IEnumerable results, DataServiceOperationContext operationContext)
        {
            WebUtil.CheckArgumentNull(feed, "feed");
            Debug.Assert(results != null, "results != null");
            Debug.Assert(operationContext != null, "operationContext != null");
            this.Feed = feed;
            this.Results = results;
            this.OperationContext = operationContext;
        }

        /// <summary>
        /// Gets the instance of ODataFeed that is going to get serialized.
        /// </summary>
        public ODataFeed Feed
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the IEnumerable instance which represent the collection that is getting serialized.
        /// </summary>
        public IEnumerable Results
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the instance of DataServiceOperationContext.
        /// </summary>
        public DataServiceOperationContext OperationContext
        {
            get;
            private set;
        }
    }
}
