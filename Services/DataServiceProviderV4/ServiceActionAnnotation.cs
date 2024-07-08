// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace DataServiceProviderV4
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service.Providers;

    /// <summary>Helper class for extension methods on the <see cref="ServiceAction"/>.</summary>
    internal static class ServiceActionExtensions
    {
        /// <summary>Helper method to get annotation from the specified service action.</summary>
        /// <param name="resourceType">The service action to get annotation for.</param>
        /// <returns>The annotation for the service action or null if the service action doesn't have annotation.</returns>
        /// <remarks>We store the annotation in the <see cref="ServiceAction.CustomState"/>, so this is just a simple helper
        /// which allows strongly typed access.</remarks>
        internal static ServiceActionAnnotation GetAnnotation(this ServiceAction serviceAction)
        {
            return serviceAction.CustomState as ServiceActionAnnotation;
        }
    }

    /// <summary>Class used to annotation <see cref="ServiceAction"/> instances with DSP specific data.</summary>
    internal class ServiceActionAnnotation
    {
        /// <summary>
        /// The action to run.
        /// </summary>
        public Func<DSPUpdateProvider, IEnumerable<object>, object> Action { get; set; }
    }
}
