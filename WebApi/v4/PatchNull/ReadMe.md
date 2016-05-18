PatchNull
------------------

This sample illustrates how to patch a property with null value to an OData service.
A request:
```
PATCH /odata/Customers(0) HTTP/1.1
Host: localhost
Content-Type: application/json

{
  "FirstName": "def",
  "SurName": null		
}
```

A response:
```
{
  "@odata.context": "http://localhost/api/$metadata#Customers/$entity",
  "Id": 0,
  "FirstName": "def",
  "SurName": null
}
```

This sample is provided as part of the OData WebApi sample repository at
https://github.com/OData/ODataSamples
