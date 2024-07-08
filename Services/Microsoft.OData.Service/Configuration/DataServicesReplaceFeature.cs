// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service.Configuration
{
    using System.Configuration;

    /// <summary>
    /// Feature for allowing replace functions in url.
    /// </summary>
    public class DataServicesReplaceFunctionFeature : ConfigurationElement
    {
        /// <summary>
        /// Returns the value of the enable attribute for data services replace feature.
        /// </summary>
        [ConfigurationProperty(DataServicesConfigurationConstants.EnableAttributeName)]
        public bool Enable
        {
            get { return (bool)this[DataServicesConfigurationConstants.EnableAttributeName]; }
            set { this[DataServicesConfigurationConstants.EnableAttributeName] = value; }
        }

        /// <summary>
        /// returns true if the element is present otherwise false.
        /// </summary>
        internal virtual bool IsPresent
        {
            get { return this.ElementInformation.IsPresent; }
        }
    }
}
