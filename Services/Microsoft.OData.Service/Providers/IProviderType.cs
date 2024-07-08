// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// Implemented by a class that encapsulates a data service provider's metadata representation of a type.
    /// </summary>
    internal interface IProviderType
    {
        /// <summary>
        /// Returns the members declared on this type only, not including any inherited members.
        /// </summary>
        IEnumerable<IProviderMember> Members { get; }

        /// <summary>
        /// Name of the type without its namespace
        /// </summary>
        string Name { get; }
    }
}
