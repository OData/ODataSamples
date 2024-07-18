// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service.Providers
{
    /// <summary>Wrapper for IDataServicePagingProvider interface discovery.</summary>
    internal sealed class DataServicePagingProviderWrapper
    {
        /// <summary>Service instance.</summary>
        private readonly IDataService service;

        /// <summary>IDataServicePagingProvider interface for the service.</summary>
        private IDataServicePagingProvider pagingProvider;

        /// <summary>Was interface already requested.</summary>
        private bool checkedForIDataServicePagingProvider;

        /// <summary>Constructor.</summary>
        /// <param name="serviceInstance">Service instance.</param>
        public DataServicePagingProviderWrapper(IDataService serviceInstance)
        {
            this.service = serviceInstance;
        }

        /// <summary>Gives reference to IDataServicePagingProvider interface implemented by the service.</summary>
        public IDataServicePagingProvider PagingProviderInterface
        {
            get
            {
                if (!this.checkedForIDataServicePagingProvider)
                {
                    this.pagingProvider = this.service.Provider.GetService<IDataServicePagingProvider>();
                    this.checkedForIDataServicePagingProvider = true;
                }

                return this.pagingProvider;
            }
        }

        /// <summary>Is custom paging enabled for the service for query processing.</summary>
        public bool IsCustomPagedForQuery
        {
            get
            {
                return this.PagingProviderInterface != null;
            }
        }

        /// <summary>Do we need to handle custom paging during serialization.</summary>
        public bool IsCustomPagedForSerialization
        {
            get { return this.checkedForIDataServicePagingProvider && this.pagingProvider != null; }
        }

        /// <summary>
        /// Dispose the pagingProvider provider instance
        /// </summary>
        internal void DisposeProvider()
        {
            if (this.pagingProvider != null)
            {
                WebUtil.Dispose(this.pagingProvider);
                this.pagingProvider = null;
            }
        }
    }
}
