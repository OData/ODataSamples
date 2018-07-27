# ODataEtagSample
--------------------------------------------------------------------

This sample shows how to use the ETag feature in ASP.NET Core OData v7.x.
The sample covers:

1) Using the If-None-Match header in GET requests.
2) Using the If-Match header in PUT requests.
3) Using the If-Match header to implement "UPSERT" semantics on the service.

This sample includes:

1. ASP.NET Core Web API Appliation depending on `Microsoft.AspNetCore.OData` nuget package.
1. .NET Framework console application used to test the Web service.

## Samples

1. Run the "ASP.NET Core Web API Application"

2. Then run the console application, when it runs, it will output the followings:

```C#
Server listening at http://localhost:5000
Retrieving a single customer at http://localhost:5000/odata/Customers(1)

StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  OData-Version: 4.0
  Date: Fri, 27 Jul 2018 21:54:25 GMT
  ETag: W/"MA=="
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 151
  Content-Type: application/json; odata.metadata=minimal; odata.streaming=true
}
{
  "@odata.context": "http://localhost:5000/odata/$metadata#Customers/$entity",
  "@odata.etag": "W/\"MA==\"",
  "Id": 1,
  "Name": "Customer 1",
  "Age": 19,
  "Version": 0
}

-----------------------------------------------------
Retrieving the customer at {0}/odata/Customers(1) when the Etag value sent matches
The response status code is: NotModified

-----------------------------------------------------
Retrieving the customer at {0}/odata/Customers(1) when the Etag value sent matches
The response status code is OK
{
  "@odata.context": "http://localhost:5000/odata/$metadata#Customers/$entity",
  "@odata.etag": "W/\"MA==\"",
  "Id": 1,
  "Name": "Customer 1",
  "Age": 19,
  "Version": 0
}

-----------------------------------------------------
Trying to update the Customer using a different ETag value on the If-Match header and failing
The response status code is PreconditionFailed

-----------------------------------------------------
Trying to update a Customer using the same ETag value on the If-Match header and succeeding

{
  "@odata.context": "http://localhost:5000/odata/$metadata#Customers/$entity",
  "@odata.etag": "W/\"MA==\"",
  "Id": 1,
  "Name": "Customer 1",
  "Age": 99,
  "Version": 0
}
-----------------------------------------------------
Trying to update a non existing customer with the If-Match header present

The response status code is NotFound

-----------------------------------------------------
Trying to update a non existing customer without the If-Match header

The response status code is Created
{
  "@odata.context": "http://localhost:5000/odata/$metadata#Customers/$entity",
  "@odata.etag": "W/\"MA==\"",
  "Id": 30,
  "Name": "New customer",
  "Age": 30,
  "Version": 0
}

-----------------------------------------------------
Press any key to exit...

```