# OData WebApi Untyped (Typeless) Sample
-----------------------------------

This sample shows how to expose an OData service without CLR types
corresponding to Edm types in the untyped model.

The sample illustrates how to create an OData service with an untyped model:

1. Create an untyped Edm model by parsing the CSDL.
2. Construct `EdmEntityObject`, `EdmEntityObjectCollection`, etc to be returned.
3. Query the Untyped OData service.

------------------

## ASP.NET Core

It's an ASP.NET Core Web Application depending on `Microsoft.AspNetCore.OData` nuget package.

When it runs, you can use any client tool (for example `POSTMAN`) to file request:

```C#
GET http://localhost:5000/odata/Movies
GET http://localhost:5000/odata/Movies(1)
GET http://localhost:5000/odata/Movies(1)/Locations
```
For example, if you file request `GET http://localhost:5000/odata/Movies(1)/Locations`

You will get:

```json
{
    "@odata.context": "http://localhost:5000/odata/$metadata#Movies(2)/Locations",
    "value": [
        {
            "@odata.type": "#NS.Address",
            "City": "Redmond",
            "Street": "28TH ST"
        },
        {
            "@odata.type": "#NS.Address",
            "City": "Issaquah",
            "Street": "Sunset ST"
        }
    ]
}
```