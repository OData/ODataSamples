# OData WebApi Singleton Sample
-----------------------------------
This sample demonstrates OData Singleton in Web API.

## Singlten Sample

This sample includes two projects:

1. .NET Framework Web Application

2. .NET Framework Console Application

------------------
### .NET Framework Web Application

It's .NET Framework Web application hosting a Web API service depending on `Microsoft.AspNet.OData` nuget package.


### .NET Framework Console Application

It's .NET Framework console application using ODataClient nuget package.

When it runs (make sure run the Web Application first) , it will output:

```C#

Please make sure the ODataSingletonSample sevice is started...
Once started, press any key to continue...

Company name is: Umbrella
Company revenue is: 1000
After update Company name by PATCH, Company name is: Umbrella-NewName
After update Company revenue by PUT, Company revenue is: 1200
After adding employees to Company, Company employees count is: Microsoft.OData.Client.DataServiceQuerySingle`1[System.Int32]
Company Employees names are:
NewHired1
NewHired2
Execute: GET http://localhost:2287/odata/Umbrella?$select=Name,Revenue
Execute: GET http://localhost:2287/odata/Umbrella?$expand=Employees
Delete navigation link: DELETE http://localhost:2287/odata/Umbrella/Employees/$ref?$id=http://localhost:50268/odata/Employees(1111)
After deleting employees of Company, Company employees count is: Microsoft.OData.Client.DataServiceQuerySingle`1[System.Int32]
Associate Company to Employees(1)
Employees(1)'s Company is: Umbrella-NewName

Press any key to quit...

```