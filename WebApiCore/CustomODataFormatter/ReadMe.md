# CustomODataFormatterSample
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

## .NET Core

It's an ASP.NET Core Web Application depending on `Microsoft.AspNetCore.OData` nuget package.
When it runs, you can use any client tool (for example `POSTMAN`) to file request:

```C#
GET http://localhost:5000/odata/Documents?search=cat

Content-Type:application/json
Prefer:odata.include-annotations=*
```

You can get the following repsonse:
```json
{
    "@odata.context": "http://localhost:5000/odata/$metadata#Documents",
    "value": [
        {
            "@org.northwind.search.score": 0.552992985375688,
            "ID": 0,
            "Name": "ReadMe.txt",
            "Content": "Lorem ipsum dolor cat amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut "
        },
        {
            "@org.northwind.search.score": 0.77911637294065039,
            "ID": 1,
            "Name": "Another.txt",
            "Content": "labore cat dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco "
        }
    ]
}
```

Be noted: the score value maybe vary at your side.


