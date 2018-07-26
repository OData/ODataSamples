# ODataOpenTypeSample
------------------

Entity or complex types which allow clients to persist additional undeclared properties are called open types. The additional undeclared properties are called dynamic properties.

This sample illustrates:

1. Build the edm model with Open types (Entity type and Complex type)
2. Add/update an open entity object with open a complex type property.
3. CRUD open entities.

------------------

## ASP.Net Core

It's an ASP.NET Core Web Application depending on `Microsoft.AspNetCore.OData` nuget package.

When it runs, you can use any client tool (for example `POSTMAN`) to file request:

```C#
GET http://localhost:5000/odata/Accounts(1)
```

You will get the following result:

```json
{
    "@odata.context": "http://localhost:5000/odata/$metadata#Accounts/$entity",
    "Id": 1,
    "Name": "Name1",
    "Gender@odata.type": "#ODataOpenTypeSample.Gender",
    "Gender": "Male",
    "Emails@odata.type": "#Collection(String)",
    "Emails": [
        "a@a.com",
        "b@b.com"
    ],
    "Address": {
        "City": "Redmond",
        "Street": "1 Microsoft Way",
        "Country": "US"
    }
}
```