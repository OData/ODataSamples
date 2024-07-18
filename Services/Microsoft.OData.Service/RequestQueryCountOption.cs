// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service
{
    /// <summary>
    /// Query Counting Option
    /// </summary>
    internal enum RequestQueryCountOption
    {
        /// <summary>Do not count the result set</summary>
        None,

        /// <summary>
        /// Count and return value (together with data)
        /// Example: http://host/service/Products?$count=true
        /// </summary>
        CountQuery,

        /// <summary>
        /// Count and return value only (as separate integer)
        /// Example: http://host/service/Categories(1)/Products/$count
        /// </summary>
        CountSegment
    }
}
