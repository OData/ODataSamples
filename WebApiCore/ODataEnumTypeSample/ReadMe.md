# ODataEnumTypeSample
------------------

This sample implements a very simple employee management system, which can query, add employees and add access level to them.

It illustrates how to use Enum Type within an OData service. Such as:
1. Enum Type Properties
2. Enum Type is used in $filter
3. Enum Type is used as a function parameter type
4. Enum Type is used as a return type of an action

------------------

## ASP.NET Core

It's an ASP.NET Core Web Application depending on `Microsoft.AspNetCore.OData` nuget package.

When it runs, you can use any client tool (for example `POSTMAN`) to file request:

```C#
http://localhost:5000/odata/Employees?$filter=Gender eq ODataEnumTypeSample.Gender'Female'
```

You will get the following result:

```json
{
    "@odata.context": "http://localhost:5000/odata/$metadata#Employees",
    "value": [
        {
            "ID": 1,
            "Name": "Lisa",
            "Gender": "Female",
            "AccessLevel": "Execute"
        },
        {
            "ID": 3,
            "Name": "Alice",
            "Gender": "Female",
            "AccessLevel": "Read, Write"
        }
    ]
}
```