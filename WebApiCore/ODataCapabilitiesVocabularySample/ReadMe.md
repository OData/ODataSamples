# ODataCapabilitiesVocabularySample
-----------------------------------

This sample illustrates capabilities vocabulary annotation support in Web API.

------------------

## ASP.NET Core

It's an ASP.NET Core Web Application depending on `Microsoft.AspNetCore.OData` nuget package.

When it runs, you can use any client tool (for example `POSTMAN`) to file request:

```C#
GET http://localhost:5000/odata/non-cap/$metadata
GET http://localhost:5000/odata/cap/$metadata
```

Also, you can query a customer as:

Query customer:

```C#
GET http://localhost:5000/odata/non-cap/Customers
GET http://localhost:5000/odata/cap/Customers
```

Query:

```C#
GET http://localhost:5000/odata/non-cap/Customers?$expand=Orders (successful)
GET http://localhost:5000/odata/cap/Customers?$expand=Orders   (failed)
```