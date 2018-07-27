# OData WebApi Singleton Sample
-----------------------------------
This sample demonstrates OData Singleton in Web API.

## Singlten Sample

This sample includes three projects:

1. ASP.NET Core Web Application

2. .NET Core Console Application

3. .NET Framework Console Application

------------------
### ASP.NET Core Core Web Application

It's ASP.NET Core Web application hosting a Web API service depending on `Microsoft.AspNetCore.OData` nuget package.

### .NET Core Console Application

It's .NET Core console application using ODataClient nuget package.

So far, it looks can't work.


### .NET Framework Console Application

It's .NET Framework console application using ODataClient nuget package.

When it runs (make sure run the Web Application first) , it will output:

```C#

Please make sure the ODataSingletonSample sevice is started...
Once started, press any key to continue...

Company name is: Umbrella
Company revenue is: 1000
After update Company name by PATCH, Company name is: Umbrella-NewName
After update Company revenue by PUT, Company revenue is: 0
After adding employees to Company, Company employees count is: Microsoft.OData.Client.DataServiceQuerySingle`1[System.Int32]
Company Employees names are:
NewHired1
NewHired2
Execute: GET http://localhost:2629/odata/Umbrella?$select=Name,Revenue
Execute: GET http://localhost:2629/odata/Umbrella?$expand=Employees
Delete navigation link: DELETE http://localhost:2629/odata/Umbrella/Employees/$ref?$id=http://localhost:50268/odata/Employees(1111)
After deleting employees of Company, Company employees count is: Microsoft.OData.Client.DataServiceQuerySingle`1[System.Int32]
Associate Company to Employees(1)
Employees(1)'s Company is:

Press any key to quit...


```