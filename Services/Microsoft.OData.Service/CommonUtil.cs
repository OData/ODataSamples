﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

#if ASTORIA_CLIENT
namespace Microsoft.OData.Client
#else
namespace Microsoft.OData.Service
#endif
{
    using System;
    using System.Threading;
    
    /// <summary>
    /// Common defintions and functions for ALL product assemblies
    /// </summary>
    internal static partial class CommonUtil
    {
        // Only StackOverflowException & ThreadAbortException are sealed classes.

        /// <summary>Type of OutOfMemoryException.</summary>
        private static readonly Type OutOfMemoryType = typeof(OutOfMemoryException);

#if !PORTABLELIB
        /// <summary>Type of StackOverflowException.</summary>
        private static readonly Type StackOverflowType = typeof(StackOverflowException);

        /// <summary>Type of ThreadAbortException.</summary>
        private static readonly Type ThreadAbortType = typeof(ThreadAbortException);
#endif

        /// <summary>
        /// Determines whether the specified exception can be caught and 
        /// handled, or whether it should be allowed to continue unwinding.
        /// </summary>
        /// <param name="e"><see cref="Exception"/> to test.</param>
        /// <returns>
        /// true if the specified exception can be caught and handled; 
        /// false otherwise.
        /// </returns>
        internal static bool IsCatchableExceptionType(Exception e)
        {
            if (e == null)
            {
                return true;
            }

            // a 'catchable' exception is defined by what it is not.
            Type type = e.GetType();
            return (
#if !PORTABLELIB
                    (type != ThreadAbortType) &&
                    (type != StackOverflowType) &&
#endif
                    (type != OutOfMemoryType));
        }
    }
}
