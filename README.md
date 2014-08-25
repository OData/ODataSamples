Sample Service
=============

##Introduction

This project is a sample service implemented by Web API OData for OData V4. It works as a reference for anyone who wants to implementate their own OData services using Web API. The underlying model for this service is TripPin which is designed to cover almost every important features OData V4 and to be as real as possible. The class diagram for TripPin is below

![Class diagram for TripPin](https://github.com/OData/SampleService/blob/master/Images/TripPinClassDiagram.jpg)

We have a [read-write TripPin service online](http://services.odata.org/V4/TripPinServiceRW), but this service is implemented directly using ODL meaning that some features in this live service may not implemented in the Web API implementation yet. But you can still take a look at this service first to take a first look at TripPin.

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

As mentioned aboved, not all features in [TripPin service online](http://services.odata.org/V4/TripPinServiceRW) are implemented since some features are still not supported in current version of Web API for OData. The features not supported yet are:
* Media entity
* Derived complex type
* .etc.
