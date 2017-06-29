ODataUntypedSample
--------------------

This sample shows how to expose an OData service without Clr types
corresponding to Edm types in the untyped model.

The sample illustrates how to create an OData service with an untyped model.
1. Create an untyped Edm model with Api provided by OData.
2. Get the Edm type from request.
3. Construct EdmEntityObject or EdmEntityObjectCollection to be returned.

There are two projects,

### ODataUntypedSample

In which, it refers to Web API OData Version 5.7 and ODataLib 6.13.
  
### ODataUntypedSample6.x

In which, it refers to Web API OData Version 6.0 and ODataLib 7.0.

--------------------
This sample is provided as part of the OData WebApi sample repository at
https://github.com/OData/ODataSamples