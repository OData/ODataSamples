// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace ODataWebV4
{
    #region Namespaces

    using Microsoft.OData.Service;
    using System.IO;
    using System.Web;

    #endregion

    /// <summary>
    /// Utility Functions for ODataWeb
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Resolve the physical path of files related to the application root
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static string ResolvePhysicalPath(string relativePath)
        {
            if (HttpContext.Current == null)
            {
                throw new DataServiceException(500, "Cannot create database. Unable to resolve physical path to the script.");
            }

            string appRoot = HttpContext.Current.Request.PhysicalApplicationPath;
            return Path.Combine(appRoot, relativePath);
        }
    }
}
