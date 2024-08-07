﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.OData.Service
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Web;
    using Microsoft.OData.Core;
    #endregion Namespaces

    /// <summary>
    /// Keeps track of the request and response headers for each
    /// operation in the batch
    /// </summary>
    internal class BatchServiceHost : IDataServiceHost2, IDisposable
    {
        #region Private fields

        /// <summary>Request Stream.</summary>
        private readonly Stream requestStream;

        /// <summary>Content Id for this operation.</summary>
        private readonly string contentId;

        /// <summary>Output writer.</summary>
        private readonly ODataBatchWriter writer;

        /// <summary>Gets the absolute URI to the resource upon which to apply the request.</summary>
        private readonly Uri absoluteRequestUri;

        /// <summary>Gets the absolute URI to the service.</summary>
        private readonly Uri absoluteServiceUri;

        /// <summary>Request Http Method</summary>
        private readonly string requestHttpMethod;

        /// <summary>Collection of request headers for the current batch operation.</summary>
        private readonly WebHeaderCollection requestHeaders;

        /// <summary>Collection of response headers for the current batch operation.</summary>
        private readonly WebHeaderCollection responseHeaders;

        /// <summary>List of query parameters as specified in the request uri.</summary>
        private NameValueCollection queryParameters;

        /// <summary>Value of the response StatusCode header.</summary>
        private int responseStatusCode;

        /// <summary>IODataResponseMessage for this operation request.</summary>
        private ODataBatchOperationResponseMessage operationMessage;

        #endregion Private fields

        #region Constructors

        /// <summary>
        /// Initializes a new dummy host for the batch request.
        /// This host represents a single operation in the batch.
        /// </summary>
        /// <param name="absoluteServiceUri">Absolute Uri to the service.</param>
        /// <param name="operationMessage">The request message representing a batch operation to wrap.</param>
        /// <param name="contentId">Content id for the given operation host.</param>
        /// <param name="writer">ODataBatchWriter instance.</param>
        /// <param name="odataMaxVersion">OData-MaxVersion header on the batch request. If the OData-MaxVersion header is not specified in the current operation, we default to the MaxDSV from the batch level.</param>
        internal BatchServiceHost(Uri absoluteServiceUri, IODataRequestMessage operationMessage, string contentId, ODataBatchWriter writer, Version odataMaxVersion)
            : this(writer)
        {
            Debug.Assert(absoluteServiceUri != null && absoluteServiceUri.IsAbsoluteUri, "absoluteServiceUri != null && absoluteServiceUri.IsAbsoluteUri");
            Debug.Assert(operationMessage != null, "operationMessage != null");
            this.absoluteServiceUri = absoluteServiceUri;
            this.absoluteRequestUri = RequestUriProcessor.GetAbsoluteUriFromReference(operationMessage.Url, absoluteServiceUri);

            this.requestHttpMethod = operationMessage.Method;
            this.contentId = contentId;

            foreach (KeyValuePair<string, string> header in operationMessage.Headers)
            {
                this.requestHeaders.Add(header.Key, header.Value);
            }

            // If the MaxDSV header is not specified in the current operation, we default to the MaxDSV from the batch level.
            if (string.IsNullOrEmpty(this.requestHeaders[XmlConstants.HttpODataMaxVersion]))
            {
                Debug.Assert(odataMaxVersion != null, "odataMaxVersion != null");
                this.requestHeaders[XmlConstants.HttpODataMaxVersion] = odataMaxVersion.ToString();
            }

            this.requestStream = operationMessage.GetStream();
        }

        /// <summary>
        /// Private constructor code that is common between normal and error construction code.
        /// </summary>
        /// <param name='writer'>ODataBatchWriter instance.</param>
        private BatchServiceHost(ODataBatchWriter writer)
        {
            Debug.Assert(writer != null, "writer != null");

            this.writer = writer;
            this.requestHeaders = new WebHeaderCollection();
            this.responseHeaders = new WebHeaderCollection();
        }

        #endregion Constructors

        #region Properties

        /// <summary>Gets the absolute URI to the resource upon which to apply the request.</summary>
        Uri IDataServiceHost.AbsoluteRequestUri
        {
            [DebuggerStepThrough]
            get { return this.absoluteRequestUri; }
        }

        /// <summary>Gets the absolute URI to the service.</summary>
        Uri IDataServiceHost.AbsoluteServiceUri
        {
            [DebuggerStepThrough]
            get { return this.absoluteServiceUri; }
        }

        /// <summary>
        /// Gets the character set encoding that the client requested,
        /// possibly null.
        /// </summary>
        string IDataServiceHost.RequestAccept
        {
            get { return this.requestHeaders[HttpRequestHeader.Accept]; }
        }

        /// <summary>
        /// Gets the character set encoding that the client requested,
        /// possibly null.
        /// </summary>
        string IDataServiceHost.RequestAcceptCharSet
        {
            get { return this.requestHeaders[HttpRequestHeader.AcceptCharset]; }
        }

        /// <summary>Gets the HTTP MIME type of the input stream.</summary>
        string IDataServiceHost.RequestContentType
        {
            get { return this.requestHeaders[HttpRequestHeader.ContentType]; }
        }

        /// <summary>
        /// Gets the HTTP data transfer method (such as GET, POST, or HEAD) used by the client.
        /// </summary>
        string IDataServiceHost.RequestHttpMethod
        {
            [DebuggerStepThrough]
            get { return this.requestHttpMethod; }
        }

        /// <summary>Gets the value of the If-Match header from the request made</summary>
        string IDataServiceHost.RequestIfMatch
        {
            get { return this.requestHeaders[HttpRequestHeader.IfMatch]; }
        }

        /// <summary>Gets the value of the If-None-Match header from the request made</summary>
        string IDataServiceHost.RequestIfNoneMatch
        {
            get { return this.requestHeaders[HttpRequestHeader.IfNoneMatch]; }
        }

        /// <summary>Gets the value for the OData-MaxVersion request header.</summary>
        string IDataServiceHost.RequestMaxVersion
        {
            get { return this.requestHeaders[XmlConstants.HttpODataMaxVersion]; }
        }

        /// <summary>Gets the value for the OData-Version request header.</summary>
        string IDataServiceHost.RequestVersion
        {
            get { return this.requestHeaders[XmlConstants.HttpODataVersion]; }
        }

        /// <summary>Gets or sets the Cache-Control header on the response.</summary>
        string IDataServiceHost.ResponseCacheControl
        {
            get { return this.responseHeaders[HttpResponseHeader.CacheControl]; }
            set { this.responseHeaders[HttpResponseHeader.CacheControl] = value; }
        }

        /// <summary>Gets or sets the HTTP MIME type of the output stream.</summary>
        string IDataServiceHost.ResponseContentType
        {
            get { return this.responseHeaders[HttpResponseHeader.ContentType]; }
            set { this.responseHeaders[HttpResponseHeader.ContentType] = value; }
        }

        /// <summary>Gets/Sets the value of the ETag header on the outgoing response</summary>
        string IDataServiceHost.ResponseETag
        {
            get { return this.responseHeaders[HttpResponseHeader.ETag]; }
            set { this.responseHeaders[HttpResponseHeader.ETag] = value; }
        }

        /// <summary>Gets or sets the Location header on the response.</summary>
        string IDataServiceHost.ResponseLocation
        {
            get { return this.responseHeaders[HttpResponseHeader.Location]; }
            set { this.responseHeaders[HttpResponseHeader.Location] = value; }
        }

        /// <summary>
        /// Gets/Sets the status code for the request made.
        /// </summary>
        int IDataServiceHost.ResponseStatusCode
        {
            get { return this.responseStatusCode; }
            set { this.responseStatusCode = value; }
        }

        /// <summary>
        /// Gets the <see cref="Stream"/> to be written to send a response
        /// to the client.
        /// </summary>
        Stream IDataServiceHost.ResponseStream
        {
            get
            {
                // There is a batch stream for writing requests for batch operations.
                // Hence this method should never be called.
                throw Error.NotSupported();
            }
        }

        /// <summary>Gets or sets the value for the OData-Version response header.</summary>
        string IDataServiceHost.ResponseVersion
        {
            get { return this.responseHeaders[XmlConstants.HttpODataVersion]; }
            set { this.responseHeaders[XmlConstants.HttpODataVersion] = value; }
        }

        /// <summary>
        /// Gets the <see cref="Stream"/> from which the request data can be read from
        /// to the client.
        /// </summary>
        Stream IDataServiceHost.RequestStream
        {
            [DebuggerStepThrough]
            get { return this.requestStream; }
        }

        #region IDataServiceHost2 Properties

        /// <summary>Dictionary of all request headers.</summary>
        WebHeaderCollection IDataServiceHost2.RequestHeaders
        {
            get { return this.requestHeaders; }
        }

        /// <summary>Enumerates all response headers that has been set.</summary>
        WebHeaderCollection IDataServiceHost2.ResponseHeaders
        {
            get { return this.responseHeaders; }
        }

        #endregion IDataServiceHost2 Properties

        /// <summary>
        /// Gets/Sets the content id as specified in the batch request.
        /// This same value is written out in the response headers also to allow mapping requests on the client.
        /// </summary>
        internal string ContentId
        {
            get { return this.contentId; }
        }

        /// <summary>Output writer.</summary>
        internal ODataBatchWriter Writer
        {
            get { return this.writer; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Disposes the object and all its resources.
        /// </summary>
        public void Dispose()
        {
            if (this.requestStream != null)
            {
                this.requestStream.Dispose();
            }
        }

        /// <summary>Gets the value for the specified item in the request query string.</summary>
        /// <param name="item">Item to return.</param>
        /// <returns>
        /// The value for the specified item in the request query string;
        /// null if <paramref name="item"/> is not found.
        /// </returns>
        string IDataServiceHost.GetQueryStringItem(string item)
        {
            this.GetUriAndQueryParameters();

            string[] result = this.queryParameters.GetValues(item);
            if (result == null || result.Length == 0)
            {
                return null;
            }

            if (result.Length == 1)
            {
                return result[0];
            }

            throw DataServiceException.CreateBadRequestError(
                Strings.DataServiceHost_MoreThanOneQueryParameterSpecifiedWithTheGivenName(item, this.absoluteRequestUri));
        }

        /// <summary>Method to handle a data service exception during processing.</summary>
        /// <param name="args">Exception handling description.</param>
        void IDataServiceHost.ProcessException(HandleExceptionArgs args)
        {
            // This would typically set headers on the host.
            WebUtil.CheckArgumentNull(args, "args");
            Debug.Assert(CommonUtil.IsCatchableExceptionType(args.Exception), "CommonUtil.IsCatchableExceptionType(args.Exception)");
            this.responseStatusCode = args.ResponseStatusCode;
            this.responseHeaders[HttpResponseHeader.ContentType] = args.ResponseContentType;
            this.responseHeaders[HttpResponseHeader.Allow] = args.ResponseAllowHeader;
        }

        /// <summary>
        /// Initializes a host for error scenarios - something to which we can write the response header values
        /// and write them to the underlying stream.
        /// </summary>
        /// <param name='writer'>ODataBatchWriter instance.</param>
        /// <returns>New BatchServiceHost instance for the error.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller will dispose the BatchWriter later.")]
        internal static BatchServiceHost CreateBatchServiceHostForError(ODataBatchWriter writer)
        {
            BatchServiceHost host = new BatchServiceHost(writer) { queryParameters = new NameValueCollection() };
            return host;
        }

        /// <summary>
        /// Return the response message for this operation.
        /// </summary>
        /// <returns>ODataBatchOperationResponseMessage instance for this batch operation.</returns>
        internal ODataBatchOperationResponseMessage GetOperationResponseMessage()
        {
            return this.operationMessage ?? (this.operationMessage = this.writer.CreateOperationResponseMessage(this.contentId));
        }

        /// <summary>
        /// Given the request uri, parse the uri and query parameters and cache them
        /// </summary>
        private void GetUriAndQueryParameters()
        {
            if (this.queryParameters == null)
            {
                this.queryParameters = HttpUtility.ParseQueryString(this.absoluteRequestUri.Query);
            }
        }

        #endregion Methods
    }
}
