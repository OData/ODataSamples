// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service
{
    #region Namespaces

    using System;
    using System.ServiceModel.Web;

    #endregion Namespaces

    /// <summary>
    /// This structure supports the .NET Framework infrastructure and is 
    /// not intended to be used directly from your code.
    /// </summary>
    /// <internal>
    /// Provides a host for services of type DataService.
    /// </internal>
    public class DataServiceHost : WebServiceHost
    {
        /// <summary>Instantiates <see cref="T:Microsoft.OData.Service.DataServiceHost" /> for WCF Data Services.</summary>
        /// <param name="serviceType">Identifies the WCF Data Services to the host.</param>
        /// <param name="baseAddresses">The URI of the host.</param>
        public DataServiceHost(Type serviceType, Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
