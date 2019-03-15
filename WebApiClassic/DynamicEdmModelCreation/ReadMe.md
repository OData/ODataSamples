# DynamicEdmModelCreation
-----------------------

This sample shows how to dynamically create an EDM model, and bind it into Web API OData pipeline.

## Background

By default, Web API OData supports a static EDM model per OData route.
In WebApiConfig.Register static method, before calling config.Routes.MapODataServiceRoute, an EDM model is created, and then assigned as a parameter to the
MapODataServiceRoute method.

```C#
public static class WebApiConfig
{
   public static void Register(HttpConfiguration config)
   {
      var builder = new ODataConventionModelBuilder();
      builder.EntitySet<Customer>("Customers");
      config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    }
}
```
However, there are some other scenarios which require runtime EDM model binding, e.g.:
1. Multi-tenant OData service: One-model per tenant
2. Hundreds of models, want to delay load as many as possible in WebApiConfig
3. Per request model

This sample describes how to create a per-request EDM model, which can be used as a foundation for scenario 1 and 2:
In request Uri, there is a path segment between route prefix and entity set, e.g. `mydatasource` in 
http://servername/odata/mydatasource/Products. Based on datasource, the service could build EDM model on the fly,
and then use an untyped controller to handle the request.
The steps are:
1. Create a customized `ODataPathRouteConstraint`, which allows to set EdmModel property, before Match is called.
2. Create a customized `ODataRoute` to override GetVirtualPath logic, and generate OData links correctly.
3. Create a customized `MapODataServiceRoute` that takes a Func<HttpRequestMessage, IEdmModel> instead of an IEdmModel.

---

## .NET Classic

It's .NET Framework Console application hosting a Web API service depending on `Microsoft.AspNet.OData` nuget package.

When it runs, it will output the following result:

```json
Server listening at http://localhost:54321
Sending request to: /odata/mydatasource/. Executing Query service document....

Result:
OK
{"@odata.context":"http://localhost:54321/odata/mydatasource/$metadata","value":[{"name":"Products","kind":"EntitySet","url":"Products"},{"name":"DetailInfos","kind":"EntitySet","url":"DetailInfos"}]}

Sending request to: /odata/mydatasource/$metadata. Executing Query $metadata....

Result:
OK
<?xml version="1.0" encoding="utf-8"?><edmx:Edmx Version="4.0" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx"><edmx:DataServices><Schema Namespace="ns" xmlns="http://docs.oasis-open.org/odata/ns/edm"><EntityType Name="Product"><Key><PropertyRef Name="ID" /></Key><Property Name="Name" Type="Edm.String" /><Property Name="ID" Type="Edm.Int32" /><NavigationProperty Name="DetailInfo" Type="ns.DetailInfo" Nullable="false" /></EntityType><EntityType Name="DetailInfo"><Key><PropertyRef Name="ID" /></Key><Property Name="ID" Type="Edm.Int32" /><Property Name="Title" Type="Edm.String" /></EntityType><EntityContainer Name="container"><EntitySet Name="Products" EntityType="ns.Product"><NavigationPropertyBinding Path="DetailInfo" Target="DetailInfos" /></EntitySet><EntitySet Name="DetailInfos" EntityType="ns.Product" /></EntityContainer></Schema></edmx:DataServices></edmx:Edmx>

Sending request to: /odata/mydatasource/Products. Executing Query the Products entity set....

Result:
OK
{"@odata.context":"http://localhost:54321/odata/mydatasource/$metadata#Products","value":[{"Name":"abc","ID":1},{"Name":"def","ID":2}]}

Sending request to: /odata/mydatasource/Products(1). Executing Query a Product entry....

Result:
OK
{"@odata.context":"http://localhost:54321/odata/mydatasource/$metadata#Products/$entity","Name":"abc","ID":1}

Sending request to: /odata/anotherdatasource/. Executing Query service document....

Result:
OK
{"@odata.context":"http://localhost:54321/odata/anotherdatasource/$metadata","value":[{"name":"Students","kind":"EntitySet","url":"Students"},{"name":"Schools","kind":"EntitySet","url":"Schools"}]}

Sending request to: /odata/anotherdatasource/$metadata. Executing Query $metadata....

Result:
OK
<?xml version="1.0" encoding="utf-8"?><edmx:Edmx Version="4.0" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx"><edmx:DataServices><Schema Namespace="ns" xmlns="http://docs.oasis-open.org/odata/ns/edm"><EntityType Name="Student"><Key><PropertyRef Name="ID" /></Key><Property Name="Name" Type="Edm.String" /><Property Name="ID" Type="Edm.Int32" /><NavigationProperty Name="School" Type="ns.School" Nullable="false" /></EntityType><EntityType Name="School"><Key><PropertyRef Name="ID" /></Key><Property Name="ID" Type="Edm.Int32" /><Property Name="CreatedDay" Type="Edm.DateTimeOffset" /></EntityType><EntityContainer Name="container"><EntitySet Name="Students" EntityType="ns.Student"><NavigationPropertyBinding Path="School" Target="Schools" /></EntitySet><EntitySet Name="Schools" EntityType="ns.Student" /></EntityContainer></Schema></edmx:DataServices></edmx:Edmx>

Sending request to: /odata/anotherdatasource/Students. Executing Query the Students entity set....

Result:
OK
{"@odata.context":"http://localhost:54321/odata/anotherdatasource/$metadata#Students","value":[{"Name":"Foo","ID":100},{"Name":"Bar","ID":101}]}

Sending request to: /odata/anotherdatasource/Students(100). Executing Query a Student entry....

Result:
OK
{"@odata.context":"http://localhost:54321/odata/anotherdatasource/$metadata#Students/$entity","Name":"Foo","ID":100}

Sending request to: /odata/mydatasource/Products(1)/Name. Executing Query the name of Products(1)....

Result:
OK
{"@odata.context":"http://localhost:54321/odata/mydatasource/$metadata#Products(1)/Name","value":"abc"}

Sending request to: /odata/anotherdatasource/Students(100)/Name. Executing Query the name of Students(100)....

Result:
OK
{"@odata.context":"http://localhost:54321/odata/anotherdatasource/$metadata#Students(100)/Name","value":"Foo"}

Sending request to: /odata/mydatasource/Products(1)/DetailInfo. Executing Query the navigation property 'DetailInfo' of Products(1)....

Result:
OK
{"@odata.context":"http://localhost:54321/odata/mydatasource/$metadata#DetailInfos/ns.DetailInfo/$entity","ID":88,"Title":"abc_detailinfo"}

Sending request to: /odata/anotherdatasource/Students(100)/School. Executing Query the navigation proeprty 'School' of Students(100)....

Result:
OK
{"@odata.context":"http://localhost:54321/odata/anotherdatasource/$metadata#Schools/ns.School/$entity","ID":99,"CreatedDay":"2016-01-19T01:02:03Z"}

Press Any Key to Exit ...
```
