# OData WebApi Spatial Sample
-----------------------------------

This sample demonstrates OData Spatial type in Web API.

It illustrates how to:

1. Build the edm model with spatial types
2. Convert between DB Spatail types and Edm Spatial types
3. EF & EF Core related operations.

Be noted, EF Core doesn't support Spatial data types (see [issue](https://github.com/aspnet/EntityFrameworkCore/issues/1100))

------------------

## ASP.NET Core

It's an ASP.NET Core Web Application depending on `Microsoft.AspNetCore.OData` nuget package.

When it runs, you can use any client tool (for example `POSTMAN`) to file request:

```C#
GET http://localhost:5000/odata/Customers
```

Also, you can post a customer as:

```C#
POST http://localhost:5000/odata/Customers
Content-Type: application/json
Content:
```

```json
{
    "Location": {
                "type": "Point",
                "coordinates": [
                    4,
                    5,
                    6,
                    7
                ],
                "crs": {
                    "type": "name",
                    "properties": {
                        "name": "EPSG:4326"
                    }
                }
            },
    "Id": 9,
    "Name": "Aury"
}
```