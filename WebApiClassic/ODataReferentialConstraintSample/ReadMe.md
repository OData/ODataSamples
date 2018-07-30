# ODataReferentialConstraintSample
------------------

Referential constraint is supported in OData WebApi v4, a constraint on the keys contained in the association type means referential constraint, these kind of key is foreign key.
 
1. Be able to configure the foreign key explicitly (HasRequired,HasOptional)
2. Be able to configure the foreign key using ForeignKeyAttribute
3. Be able to configure the foreign key convention
4. Be able to configure the multi-foreign key
5. Be able to set up the action On-Delete
6. Be able to present the referential constraint in metadata document

This sample illustrates how to:

1. Build DB context with referential constraint.
2. Build the edm model with referential constraint, set up the action On-Delete.
3. Verify the referential constraint in metadata ducument.
4. Monitor the action of Cascade Delete.

It is a console application, just open the solution and hit F5 to run it.

The output should be same as below:

```C#
Listening on http://SAM-XU-REDMOND:12345

GET ~/$metadata

StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  OData-Version: 4.0
  Date: Mon, 30 Jul 2018 22:25:05 GMT
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 1476
  Content-Type: application/xml
}

<?xml version="1.0" encoding="utf-8"?>
<edmx:Edmx Version="4.0" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx">
  <edmx:DataServices>
    <Schema Namespace="ODataReferentialConstraintSample" xmlns="http://docs.oasis-open.org/odata/ns/edm">
      <EntityType Name="Customer">
        <Key>
          <PropertyRef Name="Id" />
        </Key>
        <Property Name="Id" Type="Edm.Int32" Nullable="false" />
        <Property Name="Name" Type="Edm.String" />
        <NavigationProperty Name="Orders" Type="Collection(ODataReferentialConstraintSample.Order)" />
      </EntityType>
      <EntityType Name="Order">
        <Key>
          <PropertyRef Name="OrderId" />
        </Key>
        <Property Name="OrderId" Type="Edm.Int32" Nullable="false" />
        <Property Name="OrderName" Type="Edm.String" />
        <Property Name="CustomerId" Type="Edm.Int32" />
        <NavigationProperty Name="Customer" Type="ODataReferentialConstraintSample.Customer">
          <OnDelete Action="Cascade" />
          <ReferentialConstraint Property="CustomerId" ReferencedProperty="Id" />
        </NavigationProperty>
      </EntityType>
      <Action Name="ResetDataSource" />
      <EntityContainer Name="Container">
        <EntitySet Name="Customers" EntityType="ODataReferentialConstraintSample.Customer">
          <NavigationPropertyBinding Path="Orders" Target="Orders" />
        </EntitySet>
        <EntitySet Name="Orders" EntityType="ODataReferentialConstraintSample.Order">
          <NavigationPropertyBinding Path="Customer" Target="Customers" />
        </EntitySet>
        <ActionImport Name="ResetDataSource" Action="ODataReferentialConstraintSample.ResetDataSource" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>

POST ~/ResetDataSource
StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  Date: Mon, 30 Jul 2018 22:25:06 GMT
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 0
}


GET ~/Customers(2)
StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  OData-Version: 4.0
  Date: Mon, 30 Jul 2018 22:25:06 GMT
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 110
  Content-Type: application/json; odata.metadata=minimal
}
{
  "@odata.context": "http://sam-xu-redmond:12345/odata/$metadata#Customers/$entity",
  "Id": 2,
  "Name": "Customer #2"
}

GET ~/Orders(5)
StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  OData-Version: 4.0
  Date: Mon, 30 Jul 2018 22:25:06 GMT
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 129
  Content-Type: application/json; odata.metadata=minimal
}
{
  "@odata.context": "http://sam-xu-redmond:12345/odata/$metadata#Orders/$entity",
  "OrderId": 5,
  "OrderName": "Order #5",
  "CustomerId": 2
}

DELETE ~/Customers(2)
StatusCode: 204, ReasonPhrase: 'No Content', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  Date: Mon, 30 Jul 2018 22:25:06 GMT
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 0
}


GET ~/Customers(2)
StatusCode: 404, ReasonPhrase: 'Not Found', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  Date: Mon, 30 Jul 2018 22:25:06 GMT
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 0
}


GET ~/Orders(5)
StatusCode: 404, ReasonPhrase: 'Not Found', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  Date: Mon, 30 Jul 2018 22:25:06 GMT
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 0
}


Press any key to continue...
```