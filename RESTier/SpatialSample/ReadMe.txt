RESTier SpatialSample
------------------

This sample illustrates how to support Geography type in RESTier, and it is based on Web Api OData Spatial sample.
User can practice with the sample like, (root URL is http://localhost:18384/api/spatial/)

  GET /People
  PATCH /People(key)


When retrieve the person, the geography proeprty will be retrieved.
When patch, like path with these content, the geography property will be updated.

	{
	  "PointLocation":{
		"type":"Point","coordinates":[
		  62.808019,-1.345666
		],"crs":{
		  "type":"name","properties":{
			"name":"EPSG:4326"
		  }
		}
	  }
	}

But note, the geography type can not be part of query options or request URL, and geography related function is not supported yet.
If you need these function, refer to RESTier related issues for newest status.

For a detailed description of RESTier, refer to document http://odata.github.io/RESTier/.

For the source code of RESTier, refer to https://github.com/OData/RESTier/.

Any questions, ask on [Stack Overflow](http://stackoverflow.com/questions/ask?tags=odata).

Any issues or feature requests, report on [Github issues](https://github.com/OData/RESTier/issues).

Contribution is also highly welcomed. Please refer to the [CONTRIBUTING.md](https://github.com/OData/RESTier/blob/master/.github/CONTRIBUTING.md) for more details.

