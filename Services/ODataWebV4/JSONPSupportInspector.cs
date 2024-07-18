﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace ODataWebV4
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.Text;
    using System.Xml;
    using Microsoft.OData.Service;

    class JSONPSupportInspector : IDispatchMessageInspector
    {
        // Assume utf-8, note that Data Services supports
        // charset negotation, so this needs to be more
        // sophisticated (and per-request) if clients will 
        // use multiple charsets
        private static Encoding encoding = Encoding.UTF8;

        #region IDispatchMessageInspector Members

        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            if (request.Properties.ContainsKey("UriTemplateMatchResults"))
            {
                HttpRequestMessageProperty httpmsg = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
                UriTemplateMatch match = (UriTemplateMatch)request.Properties["UriTemplateMatchResults"];

                string format = match.QueryParameters["$format"];
                bool isVerboseJson = "verbosejson".Equals(format, StringComparison.InvariantCultureIgnoreCase);
                if (isVerboseJson || "json".Equals(format, StringComparison.InvariantCultureIgnoreCase))
                {
                    // Strip out $format from the query options to avoid an error
                    // due to use of a reserved option (starts with "$")
                    match.QueryParameters.Remove("$format");

                    // Replace the Accept header so that the Data Services runtime 
                    // assumes the client asked for a JSON representation.
                    // NOTE: we replace $format=json with 'application/json' which will then either translate to verbose JSON, JSON Light or fail 
                    //       based on conneg and versioning
                    //       we replace $format=verbosejson with 'application/json;odata=verbose' which will always translate to verbose JSON.
                    if (isVerboseJson)
                    {
                        httpmsg.Headers["Accept"] = "application/json;odata=verbose, text/plain;q=0.5";
                    }
                    else
                    {
                        httpmsg.Headers["Accept"] = "application/json, text/plain;q=0.5";
                    }

                    httpmsg.Headers["Accept-Charset"] = "utf-8";

                    string callback = match.QueryParameters["$callback"];
                    if (!string.IsNullOrEmpty(callback))
                    {
                        match.QueryParameters.Remove("$callback");
                        return callback;
                    }
                }
                else if ("atom".Equals(format, StringComparison.InvariantCultureIgnoreCase))
                {
                    match.QueryParameters.Remove("$format");
                    httpmsg.Headers["Accept"] = "application/atom+xml";
                }
                else if (!String.IsNullOrEmpty(format))
                {
                    throw new DataServiceException(400, "Invalid $format query option - the only acceptable values are \"json\", \"verbosejson\" and \"atom\".");
                }
            }
            return null;
        }

        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            if (correlationState != null && correlationState is string)
            {
                // if we have a JSONP callback then buffer the response, wrap it with the
                // callback call and then re-create the response message

                string callback = (string)correlationState;

                bool bodyIsText = false;
                HttpResponseMessageProperty response = reply.Properties[HttpResponseMessageProperty.Name] as HttpResponseMessageProperty;
                if (response != null)
                {
                    string contentType = response.Headers["Content-Type"];
                    if (contentType != null)
                    {
                        // Check the response type and change it to text/javascript if we know how.
                        if (contentType.StartsWith("text/plain", StringComparison.InvariantCultureIgnoreCase))
                        {
                            bodyIsText = true;
                            response.Headers["Content-Type"] = "text/javascript;charset=utf-8";
                        }
                        else if (contentType.StartsWith("application/json", StringComparison.InvariantCultureIgnoreCase))
                        {
                            response.Headers["Content-Type"] = contentType.Replace("application/json", "text/javascript");
                        }
                    }
                }

                XmlDictionaryReader reader = reply.GetReaderAtBodyContents();
                reader.ReadStartElement();
                string content = JSONPSupportInspector.encoding.GetString(reader.ReadContentAsBase64());

                if (bodyIsText)
                {
                    // Escape the body as a string literal.
                    content = "\"" + QuoteJScriptString(content) + "\"";
                }

                content = callback + "(" + content + ");";

                Message newreply = Message.CreateMessage(MessageVersion.None, "", new Writer(content));
                newreply.Properties.CopyProperties(reply.Properties);

                reply = newreply;
            }
        }

        private static string QuoteJScriptString(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            StringBuilder builder = null;
            int startIndex = 0;
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                char ch = s[i];
                if (((((ch == '\r') || (ch == '\t')) || ((ch == '"') || (ch == '\\'))) || (((ch == '\n') || (ch < ' ')) || ((ch > '\x007f') || (ch == '\b')))) || (ch == '\f'))
                {
                    if (builder == null)
                    {
                        builder = new StringBuilder(s.Length + 6);
                    }

                    if (count > 0)
                    {
                        builder.Append(s, startIndex, count);
                    }

                    startIndex = i + 1;
                    count = 0;
                }

                switch (ch)
                {
                    case '\b':
                        builder.Append(@"\b");
                        break;
                    case '\t':
                        builder.Append(@"\t");
                        break;
                    case '\n':
                        builder.Append(@"\n");
                        break;
                    case '\f':
                        builder.Append(@"\f");
                        break;
                    case '\r':
                        builder.Append(@"\r");
                        break;
                    case '"':
                        builder.Append("\\\"");
                        break;
                    case '\\':
                        builder.Append(@"\\");
                        break;
                    default:
                        if ((ch < ' ') || (ch > '\x007f'))
                        {
                            builder.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, @"\u{0:x4}", (int)ch);
                        }
                        else
                        {
                            count++;
                        }
                        break;
                }
            }

            string result;
            if (builder == null)
            {
                result = s;
            }
            else
            {
                if (count > 0)
                {
                    builder.Append(s, startIndex, count);
                }

                result = builder.ToString();
            }

            return result;
        }

        #endregion

        class Writer : BodyWriter
        {
            private string content;

            public Writer(string content)
                : base(false)
            {
                this.content = content;
            }

            protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
            {
                writer.WriteStartElement("Binary");
                byte[] buffer = JSONPSupportInspector.encoding.GetBytes(this.content);
                writer.WriteBase64(buffer, 0, buffer.Length);
                writer.WriteEndElement();
            }
        }

    }
}
