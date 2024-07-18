// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using Microsoft.OData.Service;

    #endregion Namespaces

    /// <summary>
    /// Represents a strongly typed service that can process data-oriented 
    /// resource requests that use EntityFramework for building the data model.
    /// </summary>
    /// <typeparam name="T">The type of the store to provide resources.</typeparam>
    public abstract class EntityFrameworkDataService<T> : DataService<T>
    {
        /// <summary>
        /// Overrides the base CreateInternalProvider implementation, only supports EF6 here.
        /// </summary>
        /// <param name="dataSourceInstance">The datasource instance for the provider.</param>
        /// <returns>
        /// The internal provider to be created. 
        /// Note that this provider also need to implement <see cref="IDataServiceMetadataProvider"/> and <see cref="IDataServiceQueryProvider"/>
        /// </returns>
        protected override IDataServiceInternalProvider CreateInternalProvider(object dataSourceInstance)
        {
            return new EntityFrameworkDataServiceProvider2(this, dataSourceInstance);
        }
    }
}
