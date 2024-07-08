// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service.Providers
{
    /// <summary>
    /// Enumeration for the kind of resource key kind
    /// </summary>
    internal enum ResourceKeyKind
    {
        /// <summary> if the key property was attributed </summary>
        AttributedKey,

        /// <summary> If the key property name was equal to TypeName+ID </summary>
        TypeNameId,

        /// <summary> If the key property name was equal to ID </summary>
        Id,
    }
}
