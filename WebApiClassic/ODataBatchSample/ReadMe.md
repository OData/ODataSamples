ODataBatchSample
-----------------

This sample shows how to use request batching in multipart/mixed and JsonBatch format based on WebApi
for AspNetCore ([Microsoft.AspNet.OData](https://www.nuget.org/packages/Microsoft.AspNet.OData/)). 
Batching is a Web API feature that allows a customer to pack several API requests and send them to the Web API
service in one HTTP request and receive a single HTTP response with the response to all
their requests. This way, the client can optimize calls to the server and improve the 
latency and scalability of its service.

The sample consists of three parts:
- Sample service for batch requests. Its Edm model contains one entity set 'customers'.
- Sample client using batch in Json format.
- Sample client using batch in the multipart/mixed format.

Sample service creates test data from scratch when service is started followed by entity set 'customers' query. It drops 
the existing database and re-creats the new database using EntityFramework in SQL server v11.0.

For more information about request batching support, check out the following links:
Json format: http://docs.oasis-open.org/odata/odata-json-format/v4.01/csprd04/odata-json-format-v4.01-csprd04.html#_Toc499716905
Multipart/Mixed format: http://docs.oasis-open.org/odata/odata/v4.01/cs01/part1-protocol/odata-v4.01-cs01-part1-protocol.html#_Toc505771274

http://blogs.msdn.com/b/webdev/archive/2013/11/01/introducing-batch-support-in-web-api-and-web-api-odata.aspx
http://aspnetwebstack.codeplex.com/wikipage?title=Web%20API%20Request%20Batching&referringTitle=Specs



This sample is provided as part of the OData WebApi sample repository at
https://github.com/OData/ODataSamples