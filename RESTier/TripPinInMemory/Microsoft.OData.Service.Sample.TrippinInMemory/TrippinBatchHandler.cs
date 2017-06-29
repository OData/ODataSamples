// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData.Batch;

namespace Microsoft.OData.Service.Sample.TrippinInMemory
{
    public class TrippinBatchHandler : Microsoft.Restier.Publishers.OData.Batch.RestierBatchHandler
    {
        public TrippinBatchHandler(HttpServer httpServer)
            : base(httpServer)
        {
        }

        public override async Task<IList<ODataBatchRequestItem>> ParseBatchRequestsAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            IList<ODataBatchRequestItem> requests = await base.ParseBatchRequestsAsync(request, cancellationToken);

            foreach (ODataBatchRequestItem requestItem in requests)
            {
                OperationRequestItem operation = requestItem as OperationRequestItem;
                if (operation != null)
                {
                    operation.Request.RequestUri = Api.TrippinApi.RemoveSessionIdFromUri(operation.Request.RequestUri);
                }
                else
                {
                    ChangeSetRequestItem changeset = requestItem as ChangeSetRequestItem;
                    if (changeset != null)
                    {
                        foreach (HttpRequestMessage changesetOperation in changeset.Requests)
                        {
                            changesetOperation.RequestUri = Api.TrippinApi.RemoveSessionIdFromUri(changesetOperation.RequestUri);
                        }
                    }
                }
            }

            return requests;
        }
    }
}