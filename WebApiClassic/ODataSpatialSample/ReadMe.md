# OData WebApi Spatial Sample
-----------------------------------

This sample demonstrates OData Spatial type in Web API.

It illustrates how to:

1. Build the edm model with spatial types
2. Convert between DB Spatail types and Edm Spatial types
3. EF related operations.

------------------

## .NET Classic

It's .NET Framework Console application hosting a Web API service depending on `Microsoft.AspNet.OData` nuget package.

When it runs, it does the following requests:

1. GET ~/$metadata
2. GET ~/Customers(2)
3. GET ~/Customers?&$filter=geo.intersects(Location, geography'SRID=4326;POLYGON((0 0, 0 3, 3 3, 3 0, 0 0))')
4. POST ~/Customers?&$filter=geo.intersects(Location, geography'SRID=4326;POLYGON((0 0, 0 1.5, 1.5 1.5, 1.5 0, 0 0))')
5. PUT ~/Customers

for example, for request #4, you will get the following output:

```C#
[Request]: Customers?&$filter=geo.intersects(Location, geography'SRID=4326;POLYGON((0 0, 0 1.5, 1.5 1.5, 1.5 0, 0 0))')
[StatusCode]: OK
[Content]:
{"@odata.context":"http://localhost/odata/$metadata#Customers","value":[{"Location":{"type":"Point","coordinates":[1.0,1.0,1.0,1.0],"crs":{"type":"name","properties":{"name":"EPSG:4326"}}},"LineString":{"type":"LineString","coordinates":[[1.0,1.0],[3.0,3.0]],"crs":{"type":"name","properties":{"name":"EPSG:4326"}}},"Id":1,"Name":"Mercury"}]}

```
