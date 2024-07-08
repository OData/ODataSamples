// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service.Configuration
{
    /// <summary>
    /// Constants to be used in the configuration file.
    /// </summary>
    internal class DataServicesConfigurationConstants
    {
        /// <summary>
        /// Name of the section where features can be turned on/off
        /// </summary>
        internal const string FeaturesSectionName = "features";

        /// <summary>
        /// Element name for allowing replace functions in url feature.
        /// </summary>
        internal const string ReplaceFunctionFeatureElementName = "replaceFunction";

        /// <summary>
        /// Attribute name to enable features.
        /// </summary>
        internal const string EnableAttributeName = "enable";
    }
}
