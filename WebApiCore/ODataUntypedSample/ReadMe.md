ODataUntypedSample
--------------------

This sample shows how to expose an OData service without Clr types
corresponding to Edm types in the untyped model using WebApi 7.x for
 AspNetCore (Microsoft.AspNetCore.OData).

The sample illustrates how to create an OData service with an untyped model.
1. Create an untyped Edm model with Api provided by OData.
2. Get the Edm type from request.
3. Construct EdmEntityObject or EdmEntityObjectCollection to be returned.

--------------------
This sample is provided as part of the OData WebApi sample repository at
https://github.com/OData/ODataSamples