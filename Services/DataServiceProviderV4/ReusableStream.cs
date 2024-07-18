// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace DataServiceProviderV4
{
    using System;
    using System.IO;

    /// <summary>
    /// Memory stream that can be reused after disposing, which will simply set the position back to the start
    /// </summary>
    internal class ReusableStream : MemoryStream, IDisposable
    {
        /// <summary>
        /// Implementation of the Dispose() function
        /// </summary>
        void IDisposable.Dispose()
        {
            // Resets the stream
            this.Position = 0;
        }

        /// <summary>
        /// Resets the stream 
        /// </summary>
        /// <param name="disposing">Whether or not to dispose managed resources</param>
        protected override void Dispose(bool disposing)
        {
            ((IDisposable)this).Dispose();
        }
    }
}
