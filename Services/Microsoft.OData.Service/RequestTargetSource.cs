﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

#if ODATALIB
namespace Microsoft.OData.Core.UriParser
#else
namespace Microsoft.OData.Service
#endif
{
    /// <summary>
    /// Provides values to describe the source of the request results.
    /// </summary>
    internal enum RequestTargetSource
    {
        /// <summary>No source for data.</summary>
        /// <remarks>
        /// This value is seen when a source hasn't been determined yet, or
        /// when the source is intrinsic to the system - eg a metadata request.
        /// </remarks>
        None,

        /// <summary>An entity set provides the data.</summary>
        EntitySet,

        /// <summary>A service operation provides the data.</summary>
        ServiceOperation,

        /// <summary>A property of an entity or a complex object provides the data.</summary>
        Property,
    }
}
