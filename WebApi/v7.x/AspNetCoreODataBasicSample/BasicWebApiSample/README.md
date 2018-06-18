
# The Basic ASP.NET Core OData with EF Core Web Application Sample        

---
This sample project is a ASP.NET Core Web Application with ASP.NET Core OData and EF Core supporting.

## Tutorial

1. In VS2017, *File*->*New Project*
2. Select **.Net Core** template, select **ASP.NET Core Web Application**
3. Give a name and select **Web API** as template.
4. Install package **Microsoft.AspNetCore.OData**
5. Add "Models" folder in the project Solution Exploer
6. Add model class files into **Models**, or copy the class files from this sample
7. In Startup.cs, 
 - Add a method to build the `IEdmModel` 
 - Modify the `ConfigureServices` method
 - Modify the `Configre` method
 
 You can refer to the sample codes.
 
8. In the **Controllers** folder, rename **ValuesController** as **CustomersController**, and copy the sample codes
9. Add **OrdersControllers** into **Controllers** folder, you can copy the codes from this sample
10. Build & Run


## Query Metadata

```C#
GET http://localhost:2153/odata/$metadata
```

```xml
<?xml version="1.0" encoding="utf-8"?>
<edmx:Edmx Version="4.0" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx">
    <edmx:DataServices>
        <Schema Namespace="BasicWebApiSample.Models" xmlns="http://docs.oasis-open.org/odata/ns/edm">
            <EntityType Name="Customer">
                <Key>
                    <PropertyRef Name="Id" />
                </Key>
                <Property Name="Id" Type="Edm.Int32" Nullable="false" />
                <Property Name="Name" Type="Edm.String" />
                <Property Name="HomeAddress" Type="BasicWebApiSample.Models.Address" />
                <Property Name="FavoriateColor" Type="BasicWebApiSample.Models.Color" Nullable="false" />
                <NavigationProperty Name="Orders" Type="Collection(BasicWebApiSample.Models.Order)" />
            </EntityType>
            <EntityType Name="Order">
                <Key>
                    <PropertyRef Name="Id" />
                </Key>
                <Property Name="Id" Type="Edm.Int32" Nullable="false" />
                <Property Name="Price" Type="Edm.Decimal" Nullable="false" />
            </EntityType>
            <ComplexType Name="Address">
                <Property Name="City" Type="Edm.String" />
                <Property Name="Street" Type="Edm.String" />
            </ComplexType>
            <EnumType Name="Color">
                <Member Name="Red" Value="0" />
                <Member Name="Yellow" Value="1" />
                <Member Name="Blue" Value="2" />
                <Member Name="Green" Value="3" />
                <Member Name="Black" Value="4" />
            </EnumType>
        </Schema>
        <Schema Namespace="Default" xmlns="http://docs.oasis-open.org/odata/ns/edm">
            <EntityContainer Name="Container">
                <EntitySet Name="Customers" EntityType="BasicWebApiSample.Models.Customer">
                    <NavigationPropertyBinding Path="Orders" Target="Orders" />
                </EntitySet>
                <EntitySet Name="Orders" EntityType="BasicWebApiSample.Models.Order" />
            </EntityContainer>
        </Schema>
    </edmx:DataServices>
</edmx:Edmx>
```

## Query Entities

```C#
GET http://localhost:2153/odata/Customers
```

```json
{
    "@odata.context": "http://localhost:2153/odata/$metadata#Customers",
    "value": [
        {
            "Id": 1,
            "Name": "Customer A",
            "FavoriateColor": "Red",
            "HomeAddress": {
                "City": "Redmond",
                "Street": "156 AVE NE"
            }
        },
        {
            "Id": 2,
            "Name": "Customer B",
            "FavoriateColor": "Red",
            "HomeAddress": {
                "City": "Bellevue",
                "Street": "Main St NE"
            }
        }
    ]
}
```

## Query options

```C#
Get http://localhost:2153/odata/Customers?$expand=Orders&$count=true
```

```json
{
    "@odata.context": "http://localhost:2153/odata/$metadata#Customers",
    "@odata.count": 2,
    "value": [
        {
            "Id": 1,
            "Name": "Customer A",
            "FavoriateColor": "Red",
            "HomeAddress": {
                "City": "Redmond",
                "Street": "156 AVE NE"
            },
            "Orders": [
                {
                    "Id": 11,
                    "Price": 10.8
                },
                {
                    "Id": 12,
                    "Price": 21.6
                },
                {
                    "Id": 13,
                    "Price": 32.4
                }
            ]
        },
        {
            "Id": 2,
            "Name": "Customer B",
            "FavoriateColor": "Red",
            "HomeAddress": {
                "City": "Bellevue",
                "Street": "Main St NE"
            },
            "Orders": [
                {
                    "Id": 21,
                    "Price": 28.8
                },
                {
                    "Id": 22,
                    "Price": 14.4
                },
                {
                    "Id": 23,
                    "Price": 9.6
                },
                {
                    "Id": 24,
                    "Price": 7.2
                }
            ]
        }
    ]
}
```


