// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace DataServiceProvider
{
    using System.Data.Services.Providers;

    /// <summary>Helper class for extension methods on the <see cref="ResourceType"/>.</summary>
    internal static class ResourceTypeExtensions
    {
        /// <summary>Helper method to get annotation from the specified resource type.</summary>
        /// <param name="resourceType">The resource type to get annotation for.</param>
        /// <returns>The annotation for the resource type or null if the resource type doesn't have annotation.</returns>
        /// <remarks>We store the annotation in the <see cref="ResourceType.CustomState"/>, so this is just a simple helper
        /// which allows strongly typed access.</remarks>
        internal static ResourceTypeAnnotation GetAnnotation(this ResourceType resourceType)
        {
            return resourceType.CustomState as ResourceTypeAnnotation;
        }
    }

    /// <summary>Class used to annotate <see cref="ResourceType"/> instances with DSP specific data.</summary>
    internal class ResourceTypeAnnotation
    {
        /// <summary>The resource into which this resource type belongs.</summary>
        /// <remarks>We don't support multiple sets with the same resource type
        ///   So there's a simple mapping between the resource type and the resource set it belongs to</remarks>
        public ResourceSet ResourceSet { get; set; }
    }
}
