// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Net;
    using System.Runtime.Serialization;

    [Serializable]
    public class ODataServiceException : Exception
    {
        public ODataServiceException()
        {
        }

        public ODataServiceException(string message)
            : base(message)
        {
        }

        public ODataServiceException(HttpStatusCode statusCode, string message)
            : this(statusCode, message, null)
        {
        }

        public ODataServiceException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public ODataServiceException(HttpStatusCode statusCode, string message, Exception innerException)
            : base(message, innerException)
        {
            this.StatusCode = statusCode;
        }

        protected ODataServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.StatusCode = (HttpStatusCode)info.GetValue("StatusCode", typeof(HttpStatusCode));
        }

        public HttpStatusCode StatusCode
        {
            get;
            private set;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("StatusCode", this.StatusCode, typeof(HttpStatusCode));
        }
    }
}