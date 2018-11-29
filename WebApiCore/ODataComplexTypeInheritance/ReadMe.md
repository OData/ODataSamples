ODataComplexTypeInheritanceSample
------------------


It illustrates the complex type inheritance requests, such as:
1. Query a property which is defined in a derived complex type
2. Function that returns a base complex type
3. Action that takes in a base complex type
4. Add an entity which contains derived complex type instance
5. Add a member of base complex type to a collection.

This sample consists of two projects, one for the service and another for the client.
To exercise the sample, first start the service in ODataComplexTypeInheritance project. 
And then interact with the service using one of the followings:
- The test client in ODataComplexTypeInheritance.Test project.
- Other REST client tools such as Postman using either HTTP or HTTPS request.


This sample is provided as part of the OData WebApi sample repository at
https://github.com/OData/ODataSamples