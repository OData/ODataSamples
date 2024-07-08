// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service.Providers
{
    /// <summary>
    /// Enumeration for the kind of service operations
    /// </summary>
    internal enum OperationKind
    {
        /// <summary>V1 Service Operation</summary>
        ServiceOperation,

        /// <summary>Side-effecting operation.</summary>
        Action,
    }
}
