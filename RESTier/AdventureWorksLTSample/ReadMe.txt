ODataServiceSample
------------------

This sample illustrates how to create an simplest OData service with RESTier and without any customization logic.
It requires to be run with VS 2015 as it uses LocalDB, it will set up DB automatically, user just need to import the solution into VS 2015, build and run. In addition the OData service exposes a $metadata document which allows the data to the consumed by OData Client for .NET clients and other clients that accept the $metadata format.

The data model contains several entity sets and operations.
User can practice with the sample like, (root URL is http://localhost:5647/api/AdventureWorksLT/)
  GET /Products
  GET /Products(key)
  GET /Products?$filter=..&$orderby=..&$top=..&$skip=..
  PATCH /Products(key)
  POST /Products

Furthermore, the service exposes a Service Document (aka. $metadata document) that 
lists all the top-level entities so clients can discover them. This enables OData clients
to discover and consume OData Services exposed.

For a detailed description of RESTier, refer to document http://odata.github.io/RESTier/.

For the source code of RESTier, refer to https://github.com/OData/RESTier/.

Any questions, ask on [Stack Overflow](http://stackoverflow.com/questions/ask?tags=odata).

Any issues or feature requests, report on [Github issues](https://github.com/OData/RESTier/issues).

Contribution is also highly welcomed. Please refer to the [CONTRIBUTING.md](https://github.com/OData/RESTier/blob/master/.github/CONTRIBUTING.md) for more details.

