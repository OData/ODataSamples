Sample Service
=============

##Introduction

This project works as a sample for OData V4. It contains the implementation of the service as well as some client scenarios of this service. The underlying model for this service is TripPin which is designed to cover almost every important features OData V4 and to be as real as possible. 

##Structure of this project
###[Web API Implementation](https://github.com/OData/ODataSamples/tree/master/Scenarios/TripPin/src/webapi)
This uses ASP.NET Web API OData V4. The live service for this implementation is on http://services.odata.org/TripPinWebApiService.

###[ODataLib Implementation](https://github.com/OData/ODataSamples/tree/master/Scenarios/TripPin/src/odatalib) 
This directly uses ODataLib. The read/write live service for this implementation si on http://services.odata.org/V4/TripPinServiceRW. 

##Model design of TripPin

The class diagram for TripPin is below

![Class diagram for TripPin](https://github.com/OData/Samples/blob/master/Scenarios/TripPin/Images/TripPinClassDiagram.jpg)

We have a [read-write TripPin service online](http://services.odata.org/V4/TripPinServiceRW) which is implemented directly using ODataLib. You can refer to [Basic Tutorials](http://www.odata.org/getting-started/basic-tutorial/) and [Advanced Tutorial](http://www.odata.org/getting-started/advanced-tutorial/) to know OData V4 and this service.



##Community

###Issue tracker
To report bugs and require features, please use our [issue tracker](https://github.com/OData/SampleService/issues).

###Mailing list
For more questions about TripPin and OData, please use the [OData Mailling List](http://www.odata.org/join-the-odata-discussion/)
