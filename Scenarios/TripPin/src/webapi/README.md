Web API Implementation
=============
##Description

This is the Web API implementation of TripPin service. And there is an [online TripPin Web API service]( http://services.odata.org/TripPinWebApiService) available.

##Project Setup

This is a common .NET Web API project. We have a in-memory way to store the data, so there is no dependency on local database. The software versions used and compatible are: 
* OData packages (You just need to build the solution and don't have to install these packages manually. In case you failed to install them, you can download them from [NuGet](http://www.nuget.org/))
  * Web API OData for OData V4
  * ODataLib
* Visual Studio 2013.
* .NET 4.5.

##Run the Sample Service

This is a common .NET Web API project. So a simple way to run it is to click the F5 button. When started, you can start by getting the metadata using ___http://localhost:[port]/$metadata___. For other requests, you can refer to [Basic Tutorials](http://www.odata.org/getting-started/basic-tutorial/) and [Advanced Tutorial](http://www.odata.org/getting-started/advanced-tutorial/).

##Known Issues

You may find that there are feature gap between the service implemented by ODL and implemented by Web API. The reason is that Web API for OData is still an active project and OData team are working hard to improve it. The features not supported yet are:
* Media entity
* Derived complex type
* .etc.
