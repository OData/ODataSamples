# CustomODataFormatter Sample
------------------

This sample illustrates how to use extensiblity points in the OData Formatter and plugin custom OData 
serializers/deserializers. The sample extends the OData Formatter to add support of OData instance annotations.

The sample models a Search service where you can query a set of documents. You can POST a document to the 'Documents' 
entityset to add it to the index. You can search for documents by sending the search query. The results of the search 
query are documents that matched. Just returning the matched documents is not that useful. Giving a score (rank) for the 
match for each document would be more useful. As the score is dependant on the incoming query, it cannot be modelled as 
a property on the document. It should instead be modelled as an annotation on the document. We use the instance annotation 
"org.northwind.search.score" to capture this.

The sample has a custom resource serializer, `AnnotatingEntitySerializer`, that adds the instance annotation to `ODataResource` 
by overriding the CreateResource method. It defines a custom `ODataSerializerProvider` (`CustomODataSerializerProvider`) to 
provide AnnotatingEntitySerializer instead of `ODataResourceSerializer`. It then creates the OData formatters using this 
serializer provider and uses those formatters in the configuration.

------------------

## .NET Classic

It's .NET Framework Console application depending on `Microsoft.AspNet.OData` nuget package.
When it runs, you can get the following output:

```C#
StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  OData-Version: 4.0
  Date: Fri, 20 Jul 2018 00:03:07 GMT
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 448
  Content-Type: application/json; odata.metadata=minimal
}

{"@odata.context":"http://localhost:9000/odata/$metadata#Documents","value":[{"@org.northwind.search.score":0.77235521784627592,"ID":0,"Name":"ReadMe.txt","Content":"Lorem ipsum dolor cat amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut "},{"@org.northwind.search.score":0.69241623798963436,"ID":1,"Name":"Another.txt","Content":"labore cat dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco "}]}
```

Be noted: the score value maybe vary at your side.
