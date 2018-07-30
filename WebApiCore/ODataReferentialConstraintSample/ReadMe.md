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

It is an ASP.NET Core Web application depending on `Microsoft.AspNetCore.OData` nuget package.

When it runs, you can use any client (for example: POSTMAN) do as follows:

## Query metadata

```C#
GET ~/$metadata
```

You should get:

```xml
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
```

## CRUD Entity

You can file request as following order:

```C#
1. GET ~/Customers(2)
2. GET ~/Orders(5)
3. DELETE ~/Customers(2)
4. GET ~/Customers(2)
5. GET ~/Orders(5)
```

So, you will get result for step 1 & step 2.
But, once we remove/delelte the `Customers(2)` at step 3, we will never get similar result for step 4 and step 5.
