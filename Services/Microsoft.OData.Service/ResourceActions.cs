﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service
{
    /// <summary>Describes an action performed on a resource.</summary>
    /// <remarks>
    /// This enumeration has been patterned after the DataRowAction
    /// (http://msdn2.microsoft.com/en-us/library/system.data.datarowaction.aspx)
    /// enumeration (with a few less values).
    /// </remarks>
    [System.Flags]
    public enum UpdateOperations
    {
        /// <summary>The resource has not changed.</summary>
        None = 0x00,

        /// <summary>The resource has been added to a container.</summary>
        Add = 0x01,

        /// <summary>The resource has changed.</summary>
        Change = 0x02,

        /// <summary>The resource has been deleted from a container.</summary>
        Delete = 0x04,
    }
}
