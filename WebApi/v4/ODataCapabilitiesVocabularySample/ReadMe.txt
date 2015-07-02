ODataCapabilitiesVocabularySample
-------------------------

This sample illustrates capabilities vocabulary annotation support in Web API.

The sample implements a simple customer-order scenario. It provides two end points,

* ~/odata/non-cap/...
   -> normal Web API OData service end point without any query limitation.

* ~/odata/cap/...
   -> Web API OData service end point with many query limitation. 

The user can checkout the metadata document by apply "$metadata" after the service root.

Sample Query:

Query Metadata:
   ~/odata/non-cap/$metadata
   ~/odata/cap/$metadata

Query customer:

     ~/odata/non-cap/Customers
     ~/odata/cap/Customers

Query:
     ~/odata/non-cap/Customers?$expand=Orders (successful)
     ~/odata/cap/Customers?$expand=Orders   (failed)
     ....

This sample is provided as part of the OData WebApi sample repository at
https://github.com/OData/ODataSamples