ODataCompositeKeySample
------------------

This sample illustrates how to create an OData service consisting of entity with 
composite key and how to provide basic CRUD functionalities on it.

Samples are provided for AspNet(classic) and AspNetCore with service and test client. Other REST clients
could be used to interact with the services via HTTP and/or HTTPS.

The data model contains one entity (Person):

* Person
	- FirstName [Key]
	- LastName [Key]
	- Age

The sample contains one People controller demostrating CRUD functionalities on Person.
The controller inherits from ODataController in order to handle multiple key parameters in actions. 
It can handle these requests:

	GET /People
	GET /People(FirstName='Kate',LastName='Jones')
	POST /People
	PATCH /People(FirstName='Kate',LastName='Jones')
	PUT /People(FirstName='Kate',LastName='Jones')
	DELETE /People(FirstName='Kate',LastName='Jones')

Support of composite-key has become an integral part of WebApi AspNet(classic) and AspNetCore(core) 
routing convention for entity set.

And the model binder attribute [FromODataUri] will convert the OData URI literal to CLR type.
So you will get strong typed parameter in action:

```
public Person Get([FromODataUri] string firstName, [FromODataUri] string lastName)
```

For AspNetCore, model binder attribute [FromBody] in controller will convert the payload data to CLR type:
```
public IActionResult PostPerson([FromBody] Person person)
```

Please see [here](http://blogs.msdn.com/b/hongyes/archive/2013/02/06/asp-net-web-api-odata-support-composite-key.aspx) for backgroud information related to composite-key.


This sample is provided as part of the OData WebApi sample repository at
https://github.com/OData/ODataSamples
