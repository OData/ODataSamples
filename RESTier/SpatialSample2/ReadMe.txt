RESTier Spatial2 Sample
------------------

This sample illustrates how to support Geography type in RESTier, and it is based on Web Api OData Spatial sample.
User can practice with the sample like, (root URL is http://localhost:18384/api/spatial2/)

  GET /People
  GET /People(0)?$select=DbLocation
  PATCH /People(key)


When retrieve the person, the geography proeprty will be retrieved.
When patch, like path with these content, the geography property will be updated.

	{
	  "DbLocation":{
		"type":"Point","coordinates":[
		  62.808019,-1.345666
		],"crs":{
		  "type":"name","properties":{
			"name":"EPSG:4326"
		  }
		}
	  }
	}

The spatial type proeprty could be part of the query option, but in order to build model with Edm type from CLR with DbGeography type, Edm model builder need to be used and ODataConversionBuilder is not supporting this kind of convert yet, and geography related function is not supported yet.
If you need these function, refer to RESTier related issues for newest status.

For a detailed description of RESTier, refer to document http://odata.github.io/RESTier/.

For the source code of RESTier, refer to https://github.com/OData/RESTier/.

Any questions, ask on [Stack Overflow](http://stackoverflow.com/questions/ask?tags=odata).

Any issues or feature requests, report on [Github issues](https://github.com/OData/RESTier/issues).

Contribution is also highly welcomed. Please refer to the [CONTRIBUTING.md](https://github.com/OData/RESTier/blob/master/.github/CONTRIBUTING.md) for more details.

