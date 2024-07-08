// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace ODataWeb
{
    using System;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.SessionState;

    /// <summary>
    /// Custom Session ID manager for OData demo service
    /// </summary>
    public class ODataWebSessionIdManager : ISessionIDManager
    {
        /// <summary>
        /// The purpose of this class is to override the Validate method of a default session id manager
        /// </summary>
        internal class ODataCustomValidationSessionIdManager : SessionIDManager
        {
            /// <summary>
            /// Validate the session ID
            /// </summary>
            /// <param name="id">The ID to be validated</param>
            /// <returns>True if the id is not null or empty</returns>
            public override bool Validate(string id)
            {
                return !string.IsNullOrEmpty(id);
            }
        }

        /// <summary>
        /// The wrapped manager instance
        /// </summary>
        SessionIDManager internalManager = new ODataCustomValidationSessionIdManager();

        /// <summary>
        /// Default session ID for all requests to the northwind root
        /// </summary>
        public const string NorthwindDefaultSession = "S_NORTHWIND";

        /// <summary>
        /// Default session ID for requests to OData root that did not specified an ID
        /// </summary>
        public const string ODataReadOnlySession = "S_ODATA_RO";

        /// <summary>
        /// Session ID Token for requests to the OData service that needs a new session ID
        /// </summary>
        public const string ODataRWCreateSession = "readwrite";

        /// <summary>
        /// Create a new session ID
        /// </summary>
        /// <param name="context">The current HttpContext</param>
        /// <returns>A string that is the generated ID</returns>
        public string CreateSessionID(HttpContext context)
        {
            if (this.IsNorthwindServiceFilePath(context.Request.FilePath))
            {
                // For the northwind service, we should just return a default ID
                // note:
                // This should never happen, since get session ID will always return an ID for NW
                // As a safe guard, we leave it on.
                return NorthwindDefaultSession;
            }
            return internalManager.CreateSessionID(context);
        }

        /// <summary>
        /// Get the session id from the request
        /// </summary>
        /// <param name="context">The current HttpContext</param>
        /// <returns>A string that is the current session ID</returns>
        public string GetSessionID(HttpContext context)
        {
            if (this.IsNorthwindServiceFilePath(context.Request.FilePath))
            {
                // Requests to northwind.svc will always have the default ID
                return NorthwindDefaultSession;
            }
            else if (this.IsODataServiceFilePath(context.Request.FilePath))
            {
                return this.GetODataSessionID();
            }
            return internalManager.GetSessionID(context);
        }

        private string GetODataSessionID()
        {
            string id = HttpContext.Current.Items["AspCookielessSession"] as string;
            // Azure web site does not support header "AspFilterSessionId", so we cannot get context.Items["AspCookielessSession"]
            // for azure web site use, Headers["X-Original-URL"] format: /(S(xxx))/odata/odata.svc
            string originalUrl = HttpContext.Current.Request.Headers["X-Original-URL"];
            if (!string.IsNullOrEmpty(originalUrl))
            {
                Match match = Regex.Match(HttpContext.Current.Request.Headers["X-Original-URL"], @"/\(S\((\w+)\)\)");
                if (match.Success)
                {
                    id = match.Groups[1].Value;
                }
            }

            if (string.IsNullOrEmpty(id))
            {
                id = ODataReadOnlySession;
            }
            else if (string.Equals(id, ODataRWCreateSession, StringComparison.InvariantCultureIgnoreCase))
            {
                id = null;
            }

            return id;
        }

        /// <summary>
        /// Initialize the manager
        /// </summary>
        public void Initialize()
        {
            internalManager.Initialize();
        }

        /// <summary>
        /// Initialize the request
        /// </summary>
        /// <param name="context">The http context</param>
        /// <param name="suppressAutoDetectRedirect">Surpress auto detect redirect</param>
        /// <param name="supportSessionIDReissue">Supports session ID reissue</param>
        /// <returns>True if the request is initialized</returns>
        public bool InitializeRequest(HttpContext context, bool suppressAutoDetectRedirect, out bool supportSessionIDReissue)
        {
            return internalManager.InitializeRequest(context, suppressAutoDetectRedirect, out supportSessionIDReissue);
        }

        /// <summary>
        /// Remove the current session ID from the context
        /// </summary>
        /// <param name="context">The http context</param>
        public void RemoveSessionID(HttpContext context)
        {
            internalManager.RemoveSessionID(context);
        }

        /// <summary>
        /// Save the new session ID
        /// </summary>
        /// <param name="context">The http context</param>
        /// <param name="id">The new session ID</param>
        /// <param name="redirected">Whether the request is redirected</param>
        /// <param name="cookieAdded">Whether a cookie is added to the context</param>
        public void SaveSessionID(HttpContext context, string id, out bool redirected, out bool cookieAdded)
        {
            if (this.IsNorthwindServiceFilePath(context.Request.FilePath))
            {
                // Northwind service does not need to be redirected, it will always have the same ID
                redirected = false;
                cookieAdded = false;
                return;
            }
            else if (this.IsODataServiceFilePath(context.Request.FilePath) && id == ODataReadOnlySession)
            {
                // OData service read only session does not need to be redirected.
                redirected = false;
                cookieAdded = false;
                return;
            }

            internalManager.SaveSessionID(context, id, out redirected, out cookieAdded);
        }

        /// <summary>
        /// Validate a session ID
        /// This method is not used, the internalManager's Validate method is called instead
        /// </summary>
        /// <param name="id">The ID to be validated</param>
        /// <returns>Always returns false</returns>
        public bool Validate(string id)
        {
            return false;
        }

        private bool IsNorthwindServiceFilePath(string filePath)
        {
            const string northwindFilePath = "/Northwind/Northwind.svc";
            return string.Equals(filePath, string.Concat(HttpContext.Current.Request.ApplicationPath, northwindFilePath), StringComparison.InvariantCultureIgnoreCase);
        }

        private bool IsODataServiceFilePath(string filePath)
        {
            const string oDataFilePath = "/OData/OData.svc";
            return string.Equals(filePath, string.Concat(HttpContext.Current.Request.ApplicationPath, oDataFilePath), StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
