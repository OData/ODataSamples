# TripPinInMemory - Sample Service using Restier API

## Introduction
[TripPinInMemory](http://services.odata.org/TripPinRESTierService) is a sample service to illustrates how to create an OData service with [Restier](https://github.com/odata/restier). The data source of [TripPinInMemory](http://services.odata.org/TripPinRESTierService) is just in memory, therefore, the dlls can be easily deployed on Azure without any db configuration.

## Model
The class diagram below describes the model design for TripPinInMemory.
![TripPinInMemory ClassDiagram](http://www.odata.org/assets/TripPinClassDiagram.jpg)

## Feature Coverage

1. Read the service root

	```
	GET http://services.odata.org/TripPinRESTierService
	```

2. Read the service metadata
	
	```
	GET http://services.odata.org/TripPinRESTierService/$metadata
	```

3. Read an entity set
	
	```
	GET http://services.odata.org/TripPinRESTierService/People
	```

4. Get a single entity from an entity set
	
	```
    GET http://services.odata.org/TripPinRESTierService/People('russellwhyte')
	```

5. Get a primitive property
	
	```
    GET http://services.odata.org/TripPinRESTierService/People('russellwhyte')/FirstName
	```

6. Get the raw value of a primitive property
	
	```
    GET http://services.odata.org/TripPinRESTierService/People('russellwhyte')/FirstName/$value
	```

7. Navigate to related entities
    
	``` 
	GET http://services.odata.org/TripPinRESTierService/People('russellwhyte')/Friends('scottketchum')/AddressInfo
	```

8. Filter a collection
    
	``` 
    GET http://services.odata.org/TripPinRESTierService/People?$filter=FirstName eq 'Vincent'
	```

9. Filter on enumeration properties
    
	``` 
    GET http://services.odata.org/TripPinRESTierService/People?$filter=Gender eq Microsoft.OData.Service.Sample.TrippinInMemory.Models.PersonGender'Female'
	```

10. Filter on nested structures
    
	``` 
    GET http://services.odata.org/TripPinRESTierService/Airports?$filter=Location/City/Region eq 'California'
	```

11. Filter using logic operators
    
	``` 
    GET http://services.odata.org/TripPinRESTierService?$filter=not(contains(FirstName,'Q')) and (AddressInfo/any(ai:ai/City/Region eq 'WA') or AddressInfo/any(ai:ai/City/Region eq 'ID'))
	```

12. Filter using any/all operators
    
	``` 
    GET http://services.odata.org/TripPinRESTierService/People?$filter=Emails/any(e: endswith(e, 'contoso.com'))
	```

13. Filter using built-in functions
    
	``` 
    GET http://services.odata.org/TripPinRESTierService/People('russellwhyte')/Trips(0)/PlanItems/#Microsoft.OData.Service.Sample.TrippinInMemory.Models.Event
	```

14. Sort a collection
    
	``` 
    GET http://services.odata.org/TripPinRESTierService/People?$orderby=FirstName desc,UserName
	```

15. Client-side paging
    
	``` 
    GET http://services.odata.org/TripPinRESTierService/People?$top=2&$skip=2
	```

16. Counting the elements in a collection
    
	``` 
    GET http://services.odata.org/TripPinRESTierService/People/$count
	```

17. Expand related entities
    
	``` 
    GET http://services.odata.org/TripPinRESTierService/People?$expand=Friends,Trips
	```

18. Select the properties
    
	``` 
    GET http://services.odata.org/TripPinRESTierService/People?$select=FirstName,LastName
	```

19. Request full metadata
    
	``` 
    GET http://services.odata.org/TripPinRESTierService/People?$format=application/json;odata.metadata=full
	```

20. Singleton
    
	``` 
    GET http://services.odata.org/TripPinRESTierService/Me
	```

21. Unbound function
    
	``` 
    GET http://services.odata.org/TripPinRESTierService/GetPersonWithMostFriends
	```

22. Action

	```
	PUT http://localhost:12691/People('russellwhyte')/LastName
	Header:
		Content-Type: application/json
	Body:
		{
	    "value": "Test"
		}
	```

