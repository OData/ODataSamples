# ODataFunctionSample

This sample solution is ported from [sample for WebApi 5.7](https://github.com/OData/ODataSamples/tree/master/WebApi/v4/ODataActionsSample) by upgrading service samples to use WebApi 7.0.1.

Demonstrate service sample usage of handling Url functions, based on Microsoft.AspNet.OData (classic) and Microsoft.AspNetcore.OData.

## Sample Function Call Url:
```
GET http://localhost:9010/odata/Products/Default.MostExpensive()
GET http://localhost:9010/odata/Products/Default.Top10()
GET http://localhost:9010/odata/Products(33)/Default.GetPriceRank()
GET http://localhost:9010/odata/Products(33)/Default.CalculateGeneralSalesTax(state='WA')
GET http://localhost:9010/odata/GetSalesTaxRate(state='CA')
GET http://localhost:9010/odata/GetSalesTaxRate(state=@p1)?@p1='ND'
```

Use port 44363 for HTTPS. For example: 
```
https://localhost:44363/odata/Products/Default.MostExpensive()
```


## Sample Projects:
Data is populated during service startup and maintained in memory.
	
- **WebApi classic sample: ODataFunctionSample.AspNet**:

	Function calls are executed as part of startup. They can also be invoked interactively using REST client application (e.g. Postman) after service is up & running.

- **WebApi AspNetCore sample: ODataFunctionsample.AspNetcore**:

	Because this is a demo for function call urls, the web-app is a read-only service. After service is up & running, use REST client to interact with the service endpoints for the functions.
