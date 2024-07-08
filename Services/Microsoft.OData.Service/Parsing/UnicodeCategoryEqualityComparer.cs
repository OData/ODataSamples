// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service.Parsing
{
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>This class implements IEqualityComparer for UnicodeCategory</summary>
    /// <remarks>
    /// Using this class rather than EqualityComparer&lt;T&gt;.Default 
    /// saves from JIT'ing it in each AppDomain.
    /// </remarks>
    internal class UnicodeCategoryEqualityComparer : IEqualityComparer<UnicodeCategory>
    {
        /// <summary>
        /// Checks whether two unicode categories are equal
        /// </summary>
        /// <param name="x">first unicode category</param>
        /// <param name="y">second unicode category</param>
        /// <returns>true if they are equal, false otherwise</returns>
        public bool Equals(UnicodeCategory x, UnicodeCategory y)
        {
            return x == y;
        }

        /// <summary>
        /// Gets a hash code for the specified unicode category
        /// </summary>
        /// <param name="obj">the input value</param>
        /// <returns>The hash code for the given input unicode category, the underlying int</returns>
        public int GetHashCode(UnicodeCategory obj)
        {
            return (int)obj;
        }
    }
}
