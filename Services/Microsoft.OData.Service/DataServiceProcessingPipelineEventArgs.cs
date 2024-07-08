// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service
{
    #region Namespaces

    using System;
    using System.Diagnostics;

    #endregion Namespaces

    /// <summary>
    /// Event argument class for DataServiceProcessingPipeline events.
    /// </summary>
    public sealed class DataServiceProcessingPipelineEventArgs : EventArgs
    {
        /// <summary>
        /// Context for the operation which the current event is fired for.
        /// </summary>
        private readonly DataServiceOperationContext operationContext;

        /// <summary>
        /// Constructs a new instance of DataServicePipelineEventArgs object
        /// </summary>
        /// <param name="operationContext">Context for the operation which the current event is fired for.</param>
        internal DataServiceProcessingPipelineEventArgs(DataServiceOperationContext operationContext)
        {
            Debug.Assert(operationContext != null, "operationContext != null");
            this.operationContext = operationContext;
        }

        /// <summary>Gets the context of the operation that raised the event.</summary>
        /// <returns>A <see cref="T:Microsoft.OData.Service.DataServiceOperationContext" /> that is the operation context. </returns>
        public DataServiceOperationContext OperationContext
        {
            [DebuggerStepThrough]
            get { return this.operationContext; }
        }
    }
}
