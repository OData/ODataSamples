// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Samples.SqlServer.AdventureWorksService
{
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
        /// Default session ID for all requests to the AdventureWorks root
        /// </summary>
        public const string AdventureWorksDefaultSession = "S_ADVENTUREWORKS";

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
            if (context.Request.FilePath == "/AdventureWorksV3/AdventureWorks.svc")
            {
                // For the northwind service, we should just return a default ID
                // note:
                // This should never happen, since get session ID will always return an ID for NW
                // As a safe guard, we leave it on.
                return AdventureWorksDefaultSession;
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
            if (context.Request.FilePath == "/AdventureWorksV3/AdventureWorks.svc")
            {
                // Requests to AdventureWorks.svc will always have the default ID
                return AdventureWorksDefaultSession;
            }

            return internalManager.GetSessionID(context);
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
            if (context.Request.FilePath == "/AdventureWorksV3/AdventureWorks.svc")
            {
                // AdventureWorks service does not need to be redirected, it will always have the same ID
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
    }
}
