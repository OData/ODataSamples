// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    #endregion

    /// <summary>
    /// Internal interface whose implementation loads known types, validates the model and sets 
    /// all the metadata objects to read-only. Also used for obtaining entity container annotations.
    /// </summary>
    public interface IDataServiceInternalProvider
    {
        /// <summary>
        /// Called by the service to let the provider perform data model validation.
        /// </summary>
        /// <param name="knownTypes">Collection of known types.</param>
        /// <param name="useMetadataCacheOrder">Whether to use metadata cache ordering instead of default ordering.</param>
        void FinalizeMetadataModel(IEnumerable<Type> knownTypes, bool useMetadataCacheOrder);

        /// <summary>
        /// Return the list of custom annotation for the entity container with the given name.
        /// </summary>
        /// <param name="entityContainerName">Name of the EntityContainer.</param>
        /// <returns>Return the list of custom annotation for the entity container with the given name.</returns>
        IEnumerable<KeyValuePair<string, object>> GetEntityContainerAnnotations(string entityContainerName);
    }
}
