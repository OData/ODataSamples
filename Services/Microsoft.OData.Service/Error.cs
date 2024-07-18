// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service
{
    using System;

    /// <summary>
    /// Strongly-typed and parameterized exception factory.
    /// </summary>
    internal static partial class Error
    {
        /// <summary>
        /// create and trace a HttpHeaderFailure
        /// </summary>
        /// <param name="errorCode">error code</param>
        /// <param name="message">error message</param>
        /// <returns>DataServiceException</returns>
        internal static DataServiceException HttpHeaderFailure(int errorCode, string message)
        {
            return Trace(new DataServiceException(errorCode, message));
        }

        /// <summary>
        /// Trace the exception
        /// </summary>
        /// <typeparam name="T">type of the exception</typeparam>
        /// <param name="exception">exception object to trace</param>
        /// <returns>the exception parameter</returns>
        private static T Trace<T>(T exception) where T : Exception
        {
            return exception;
        }
    }
}
