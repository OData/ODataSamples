# ODataCapabilitiesVocabularySample
-------------------------

This sample illustrates capabilities vocabulary annotation support in Web API.

It is a .NET Framework console application depending on `Microsoft.AspNet.OData` nuget package.

It implements a simple customer-order scenario. It provides two end points,

* ~/odata/non-cap/...
   -> normal Web API OData service end point without any query limitation.

* ~/odata/cap/...
   -> Web API OData service end point with many query limitation. 

---
When it runs, it will do the following sample Query:

Query Metadata:
   ~/odata/non-cap/$metadata
   ~/odata/cap/$metadata

Query customer:

     ~/odata/non-cap/Customers
     ~/odata/cap/Customers

Query:
     ~/odata/non-cap/Customers?$expand=Orders (successful)
     ~/odata/cap/Customers?$expand=Orders   (failed)
     ....

--
Below is the output:

```C#
Listening on http://localhost:12345/
GET ~/odata/non-cap/$metadata
StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  OData-Version: 4.0
  Date: Mon, 30 Jul 2018 21:16:11 GMT
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 1748
  Content-Type: application/xml
}
<?xml version="1.0" encoding="utf-8"?>
<edmx:Edmx Version="4.0" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx">
  <edmx:DataServices>
    <Schema Namespace="CapabilitiesVocabulary" xmlns="http://docs.oasis-open.org/odata/ns/edm">
      <ComplexType Name="Address">
        <Property Name="City" Type="Edm.String" />
        <Property Name="Street" Type="Edm.String" />
      </ComplexType>
      <EntityType Name="Customer">
        <Key>
          <PropertyRef Name="CustomerId" />
        </Key>
        <Property Name="CustomerId" Type="Edm.Int32" Nullable="false" />
        <Property Name="Name" Type="Edm.String" />
        <Property Name="Token" Type="Edm.Guid" Nullable="false" />
        <Property Name="Email" Type="Edm.String" />
        <Property Name="Addresses" Type="Collection(CapabilitiesVocabulary.Address)" Nullable="false" />
        <Property Name="FavoriateColors" Type="Collection(CapabilitiesVocabulary.Color)" Nullable="false" />
        <NavigationProperty Name="Orders" Type="Collection(CapabilitiesVocabulary.Order)" />
      </EntityType>
      <EntityType Name="Order">
        <Key>
          <PropertyRef Name="OrderId" />
        </Key>
        <Property Name="OrderId" Type="Edm.Int32" Nullable="false" />
        <Property Name="Price" Type="Edm.Double" Nullable="false" />
      </EntityType>
      <EnumType Name="Color">
        <Member Name="Red" Value="0" />
        <Member Name="Green" Value="1" />
        <Member Name="Blue" Value="2" />
        <Member Name="Yellow" Value="3" />
        <Member Name="Pink" Value="4" />
        <Member Name="Purple" Value="5" />
      </EnumType>
    </Schema>
    <Schema Namespace="Default" xmlns="http://docs.oasis-open.org/odata/ns/edm">
      <EntityContainer Name="Container">
        <EntitySet Name="Customers" EntityType="CapabilitiesVocabulary.Customer">
          <NavigationPropertyBinding Path="Orders" Target="Orders" />
        </EntitySet>
        <EntitySet Name="Orders" EntityType="CapabilitiesVocabulary.Order" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>
GET ~/odata/cap/$metadata
StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  OData-Version: 4.0
  Date: Mon, 30 Jul 2018 21:16:11 GMT
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 3251
  Content-Type: application/xml
}
<?xml version="1.0" encoding="utf-8"?>
<edmx:Edmx Version="4.0" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx">
  <edmx:DataServices>
    <Schema Namespace="CapabilitiesVocabulary" xmlns="http://docs.oasis-open.org/odata/ns/edm">
      <EntityType Name="Customer">
        <Key>
          <PropertyRef Name="CustomerId" />
        </Key>
        <Property Name="CustomerId" Type="Edm.Int32" Nullable="false" />
        <Property Name="Name" Type="Edm.String" />
        <Property Name="Token" Type="Edm.Guid" Nullable="false" />
        <Property Name="Email" Type="Edm.String" />
        <Property Name="Addresses" Type="Collection(CapabilitiesVocabulary.Address)" />
        <Property Name="FavoriateColors" Type="Collection(CapabilitiesVocabulary.Color)" Nullable="false" />
        <NavigationProperty Name="Orders" Type="Collection(CapabilitiesVocabulary.Order)" />
      </EntityType>
      <EntityType Name="Order">
        <Key>
          <PropertyRef Name="OrderId" />
        </Key>
        <Property Name="OrderId" Type="Edm.Int32" Nullable="false" />
        <Property Name="Price" Type="Edm.Double" Nullable="false" />
      </EntityType>
      <ComplexType Name="Address">
        <Property Name="City" Type="Edm.String" />
        <Property Name="Street" Type="Edm.String" />
      </ComplexType>
      <EnumType Name="Color">
        <Member Name="Red" Value="0" />
        <Member Name="Green" Value="1" />
        <Member Name="Blue" Value="2" />
        <Member Name="Yellow" Value="3" />
        <Member Name="Pink" Value="4" />
        <Member Name="Purple" Value="5" />
      </EnumType>
    </Schema>
    <Schema Namespace="Default" xmlns="http://docs.oasis-open.org/odata/ns/edm">
      <EntityContainer Name="Container">
        <EntitySet Name="Customers" EntityType="CapabilitiesVocabulary.Customer">
          <NavigationPropertyBinding Path="Orders" Target="Orders" />
          <Annotation Term="Org.OData.Capabilities.V1.CountRestrictions">
            <Record>
              <PropertyValue Property="Countable" Bool="true" />
              <PropertyValue Property="NonCountableProperties">
                <Collection>
                  <PropertyPath>Addresses</PropertyPath>
                  <PropertyPath>FavoriateColors</PropertyPath>
                </Collection>
              </PropertyValue>
              <PropertyValue Property="NonCountableNavigationProperties">
                <Collection />
              </PropertyValue>
            </Record>
          </Annotation>
          <Annotation Term="Org.OData.Capabilities.V1.FilterRestrictions">
            <Record>
              <PropertyValue Property="Filterable" Bool="true" />
              <PropertyValue Property="RequiresFilter" Bool="true" />
              <PropertyValue Property="RequiredProperties">
                <Collection />
              </PropertyValue>
              <PropertyValue Property="NonFilterableProperties">
                <Collection>
                  <PropertyPath>Token</PropertyPath>
                </Collection>
              </PropertyValue>
            </Record>
          </Annotation>
          <Annotation Term="Org.OData.Capabilities.V1.SortRestrictions">
            <Record>
              <PropertyValue Property="Sortable" Bool="true" />
              <PropertyValue Property="AscendingOnlyProperties">
                <Collection />
              </PropertyValue>
              <PropertyValue Property="DescendingOnlyProperties">
                <Collection />
              </PropertyValue>
              <PropertyValue Property="NonSortableProperties">
                <Collection>
                  <PropertyPath>Token</PropertyPath>
                </Collection>
              </PropertyValue>
            </Record>
          </Annotation>
          <Annotation Term="Org.OData.Capabilities.V1.ExpandRestrictions">
            <Record>
              <PropertyValue Property="Expandable" Bool="true" />
              <PropertyValue Property="NonExpandableProperties">
                <Collection>
                  <NavigationPropertyPath>Orders</NavigationPropertyPath>
                </Collection>
              </PropertyValue>
            </Record>
          </Annotation>
        </EntitySet>
        <EntitySet Name="Orders" EntityType="CapabilitiesVocabulary.Order" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>
GET ~/odata/cap/Customers
StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  OData-Version: 4.0
  Date: Mon, 30 Jul 2018 21:16:11 GMT
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 1714
  Content-Type: application/json; odata.metadata=minimal
}
{
  "@odata.context": "http://localhost:12345/odata/non-cap/$metadata#Customers",
  "value": [
    {
      "CustomerId": 1,
      "Name": "John",
      "Token": "f83e70fc-cfab-45ef-9056-fb3d9b71e221",
      "Email": "John@microsoft.com",
      "FavoriateColors": [
        "Red",
        "Green"
      ],
      "Addresses": [
        {
          "City": "Redmond",
          "Street": "One Microsoft way"
        }
      ]
    },
    {
      "CustomerId": 2,
      "Name": "Peter",
      "Token": "7548e3a2-6bb2-4797-92c6-11008a8ffdd3",
      "Email": "Peter@microsoft.com",
      "FavoriateColors": [
        "Blue",
        "Blue"
      ],
      "Addresses": [
        {
          "City": "Shanghai",
          "Street": "ZiXing Rd"
        }
      ]
    },
    {
      "CustomerId": 3,
      "Name": "Mike",
      "Token": "f0b68809-7e49-4447-820f-6533a4e8eaf9",
      "Email": "Mike@microsoft.com",
      "FavoriateColors": [
        "Pink",
        "Yellow",
        "Purple"
      ],
      "Addresses": [
        {
          "City": "Beijing",
          "Street": "Fujian Rd"
        }
      ]
    },
    {
      "CustomerId": 4,
      "Name": "Sam",
      "Token": "bd4cdc8e-45ab-4f20-86c5-6ee838c8d554",
      "Email": "Sam@microsoft.com",
      "FavoriateColors": [
        "Green",
        "Green"
      ],
      "Addresses": [
        {
          "City": "Redmond",
          "Street": "One Microsoft way"
        },
        {
          "City": "Beijing",
          "Street": "Fujian Rd"
        }
      ]
    },
    {
      "CustomerId": 5,
      "Name": "Mark",
      "Token": "1e72cc05-406c-40fa-acf9-ad24d87b74e1",
      "Email": "Mark@microsoft.com",
      "FavoriateColors": [
        "Purple",
        "Blue"
      ],
      "Addresses": [
        {
          "City": "Redmond",
          "Street": "One Microsoft way"
        },
        {
          "City": "Beijing",
          "Street": "Fujian Rd"
        },
        {
          "City": "Shanghai",
          "Street": "ZiXing Rd"
        }
      ]
    },
    {
      "CustomerId": 6,
      "Name": "Ted",
      "Token": "e9aceeb0-de5b-42b1-a4e9-b1b9cf71994f",
      "Email": "Ted@microsoft.com",
      "FavoriateColors": [
        "Yellow",
        "Purple"
      ],
      "Addresses": [
        {
          "City": "Beijing",
          "Street": "Fujian Rd"
        },
        {
          "City": "Shanghai",
          "Street": "ZiXing Rd"
        }
      ]
    },
    {
      "CustomerId": 7,
      "Name": "Bear",
      "Token": "57b7c8e1-10bf-4eb0-9bd3-a54a51530e30",
      "Email": "Bear@microsoft.com",
      "FavoriateColors": [
        "Red",
        "Purple",
        "Green"
      ],
      "Addresses": [
        {
          "City": "Redmond",
          "Street": "One Microsoft way"
        },
        {
          "City": "Shanghai",
          "Street": "ZiXing Rd"
        }
      ]
    }
  ]
}
GET ~/odata/cap/Customers
StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  OData-Version: 4.0
  Date: Mon, 30 Jul 2018 21:16:11 GMT
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 1710
  Content-Type: application/json; odata.metadata=minimal
}
{
  "@odata.context": "http://localhost:12345/odata/cap/$metadata#Customers",
  "value": [
    {
      "CustomerId": 1,
      "Name": "John",
      "Token": "f83e70fc-cfab-45ef-9056-fb3d9b71e221",
      "Email": "John@microsoft.com",
      "FavoriateColors": [
        "Red",
        "Green"
      ],
      "Addresses": [
        {
          "City": "Redmond",
          "Street": "One Microsoft way"
        }
      ]
    },
    {
      "CustomerId": 2,
      "Name": "Peter",
      "Token": "7548e3a2-6bb2-4797-92c6-11008a8ffdd3",
      "Email": "Peter@microsoft.com",
      "FavoriateColors": [
        "Blue",
        "Blue"
      ],
      "Addresses": [
        {
          "City": "Shanghai",
          "Street": "ZiXing Rd"
        }
      ]
    },
    {
      "CustomerId": 3,
      "Name": "Mike",
      "Token": "f0b68809-7e49-4447-820f-6533a4e8eaf9",
      "Email": "Mike@microsoft.com",
      "FavoriateColors": [
        "Pink",
        "Yellow",
        "Purple"
      ],
      "Addresses": [
        {
          "City": "Beijing",
          "Street": "Fujian Rd"
        }
      ]
    },
    {
      "CustomerId": 4,
      "Name": "Sam",
      "Token": "bd4cdc8e-45ab-4f20-86c5-6ee838c8d554",
      "Email": "Sam@microsoft.com",
      "FavoriateColors": [
        "Green",
        "Green"
      ],
      "Addresses": [
        {
          "City": "Redmond",
          "Street": "One Microsoft way"
        },
        {
          "City": "Beijing",
          "Street": "Fujian Rd"
        }
      ]
    },
    {
      "CustomerId": 5,
      "Name": "Mark",
      "Token": "1e72cc05-406c-40fa-acf9-ad24d87b74e1",
      "Email": "Mark@microsoft.com",
      "FavoriateColors": [
        "Purple",
        "Blue"
      ],
      "Addresses": [
        {
          "City": "Redmond",
          "Street": "One Microsoft way"
        },
        {
          "City": "Beijing",
          "Street": "Fujian Rd"
        },
        {
          "City": "Shanghai",
          "Street": "ZiXing Rd"
        }
      ]
    },
    {
      "CustomerId": 6,
      "Name": "Ted",
      "Token": "e9aceeb0-de5b-42b1-a4e9-b1b9cf71994f",
      "Email": "Ted@microsoft.com",
      "FavoriateColors": [
        "Yellow",
        "Purple"
      ],
      "Addresses": [
        {
          "City": "Beijing",
          "Street": "Fujian Rd"
        },
        {
          "City": "Shanghai",
          "Street": "ZiXing Rd"
        }
      ]
    },
    {
      "CustomerId": 7,
      "Name": "Bear",
      "Token": "57b7c8e1-10bf-4eb0-9bd3-a54a51530e30",
      "Email": "Bear@microsoft.com",
      "FavoriateColors": [
        "Red",
        "Purple",
        "Green"
      ],
      "Addresses": [
        {
          "City": "Redmond",
          "Street": "One Microsoft way"
        },
        {
          "City": "Shanghai",
          "Street": "ZiXing Rd"
        }
      ]
    }
  ]
}
GET ~/odata/cap/Customers?$expand=Orders
StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  OData-Version: 4.0
  Date: Mon, 30 Jul 2018 21:16:11 GMT
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 2659
  Content-Type: application/json; odata.metadata=minimal
}
{
  "@odata.context": "http://localhost:12345/odata/non-cap/$metadata#Customers",
  "value": [
    {
      "CustomerId": 1,
      "Name": "John",
      "Token": "f83e70fc-cfab-45ef-9056-fb3d9b71e221",
      "Email": "John@microsoft.com",
      "FavoriateColors": [
        "Red",
        "Green"
      ],
      "Addresses": [
        {
          "City": "Redmond",
          "Street": "One Microsoft way"
        }
      ],
      "Orders": [
        {
          "OrderId": 11,
          "Price": 10.9
        }
      ]
    },
    {
      "CustomerId": 2,
      "Name": "Peter",
      "Token": "7548e3a2-6bb2-4797-92c6-11008a8ffdd3",
      "Email": "Peter@microsoft.com",
      "FavoriateColors": [
        "Blue",
        "Blue"
      ],
      "Addresses": [
        {
          "City": "Shanghai",
          "Street": "ZiXing Rd"
        }
      ],
      "Orders": [
        {
          "OrderId": 21,
          "Price": 20.8
        },
        {
          "OrderId": 22,
          "Price": 21.8
        }
      ]
    },
    {
      "CustomerId": 3,
      "Name": "Mike",
      "Token": "f0b68809-7e49-4447-820f-6533a4e8eaf9",
      "Email": "Mike@microsoft.com",
      "FavoriateColors": [
        "Pink",
        "Yellow",
        "Purple"
      ],
      "Addresses": [
        {
          "City": "Beijing",
          "Street": "Fujian Rd"
        }
      ],
      "Orders": [
        {
          "OrderId": 31,
          "Price": 30.700000000000003
        },
        {
          "OrderId": 32,
          "Price": 31.700000000000003
        },
        {
          "OrderId": 33,
          "Price": 32.7
        }
      ]
    },
    {
      "CustomerId": 4,
      "Name": "Sam",
      "Token": "bd4cdc8e-45ab-4f20-86c5-6ee838c8d554",
      "Email": "Sam@microsoft.com",
      "FavoriateColors": [
        "Green",
        "Green"
      ],
      "Addresses": [
        {
          "City": "Redmond",
          "Street": "One Microsoft way"
        },
        {
          "City": "Beijing",
          "Street": "Fujian Rd"
        }
      ],
      "Orders": [
        {
          "OrderId": 41,
          "Price": 40.6
        },
        {
          "OrderId": 42,
          "Price": 41.6
        },
        {
          "OrderId": 43,
          "Price": 42.6
        },
        {
          "OrderId": 44,
          "Price": 43.6
        }
      ]
    },
    {
      "CustomerId": 5,
      "Name": "Mark",
      "Token": "1e72cc05-406c-40fa-acf9-ad24d87b74e1",
      "Email": "Mark@microsoft.com",
      "FavoriateColors": [
        "Purple",
        "Blue"
      ],
      "Addresses": [
        {
          "City": "Redmond",
          "Street": "One Microsoft way"
        },
        {
          "City": "Beijing",
          "Street": "Fujian Rd"
        },
        {
          "City": "Shanghai",
          "Street": "ZiXing Rd"
        }
      ],
      "Orders": [
        {
          "OrderId": 51,
          "Price": 50.5
        },
        {
          "OrderId": 52,
          "Price": 51.5
        },
        {
          "OrderId": 53,
          "Price": 52.5
        },
        {
          "OrderId": 54,
          "Price": 53.5
        },
        {
          "OrderId": 55,
          "Price": 54.5
        }
      ]
    },
    {
      "CustomerId": 6,
      "Name": "Ted",
      "Token": "e9aceeb0-de5b-42b1-a4e9-b1b9cf71994f",
      "Email": "Ted@microsoft.com",
      "FavoriateColors": [
        "Yellow",
        "Purple"
      ],
      "Addresses": [
        {
          "City": "Beijing",
          "Street": "Fujian Rd"
        },
        {
          "City": "Shanghai",
          "Street": "ZiXing Rd"
        }
      ],
      "Orders": [
        {
          "OrderId": 61,
          "Price": 60.400000000000006
        },
        {
          "OrderId": 62,
          "Price": 61.400000000000006
        },
        {
          "OrderId": 63,
          "Price": 62.400000000000006
        },
        {
          "OrderId": 64,
          "Price": 63.400000000000006
        },
        {
          "OrderId": 65,
          "Price": 64.4
        },
        {
          "OrderId": 66,
          "Price": 65.4
        }
      ]
    },
    {
      "CustomerId": 7,
      "Name": "Bear",
      "Token": "57b7c8e1-10bf-4eb0-9bd3-a54a51530e30",
      "Email": "Bear@microsoft.com",
      "FavoriateColors": [
        "Red",
        "Purple",
        "Green"
      ],
      "Addresses": [
        {
          "City": "Redmond",
          "Street": "One Microsoft way"
        },
        {
          "City": "Shanghai",
          "Street": "ZiXing Rd"
        }
      ],
      "Orders": [
        {
          "OrderId": 71,
          "Price": 70.3
        },
        {
          "OrderId": 72,
          "Price": 71.3
        },
        {
          "OrderId": 73,
          "Price": 72.3
        },
        {
          "OrderId": 74,
          "Price": 73.3
        },
        {
          "OrderId": 75,
          "Price": 74.3
        },
        {
          "OrderId": 76,
          "Price": 75.3
        },
        {
          "OrderId": 77,
          "Price": 76.3
        }
      ]
    }
  ]
}
GET ~/odata/cap/Customers?$expand=Orders
***A repsonse with bad request is expected***
StatusCode: 400, ReasonPhrase: 'Bad Request', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  OData-Version: 4.0
  Date: Mon, 30 Jul 2018 21:16:11 GMT
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 1930
  Content-Type: application/json; odata.metadata=minimal
}
{
  "error": {
    "code": "",
    "message": "The query specified in the URI is not valid. The property 'Orders' cannot be used in the $expand query option.",
    "innererror": {
      "message": "The property 'Orders' cannot be used in the $expand query option.",
      "type": "Microsoft.OData.ODataException",
      "stacktrace": "   at Microsoft.AspNet.OData.Query.Validators.SelectExpandQueryValidator.ValidateRestrictions(Nullable`1 remainDepth, Int32 currentDepth, SelectExpandClause selectExpandClause, IEdmNavigationProperty navigationProperty, ODataValidationSettings validationSettings)\r\n   at Microsoft.AspNet.OData.Query.Validators.SelectExpandQueryValidator.Validate(SelectExpandQueryOption selectExpandQueryOption, ODataValidationSettings validationSettings)\r\n   at Microsoft.AspNet.OData.Query.SelectExpandQueryOption.Validate(ODataValidationSettings validationSettings)\r\n   at Microsoft.AspNet.OData.Query.Validators.ODataQueryValidator.Validate(ODataQueryOptions options, ODataValidationSettings validationSettings)\r\n   at Microsoft.AspNet.OData.Query.ODataQueryOptions.Validate(ODataValidationSettings validationSettings)\r\n   at Microsoft.AspNet.OData.EnableQueryAttribute.ValidateQuery(HttpRequestMessage request, ODataQueryOptions queryOptions)\r\n   at Microsoft.AspNet.OData.EnableQueryAttribute.<>c__DisplayClass0_0.<OnActionExecuted>b__3(ODataQueryContext queryContext)\r\n   at Microsoft.AspNet.OData.EnableQueryAttribute.ExecuteQuery(Object responseValue, IQueryable singleResultCollection, IWebApiActionDescriptor actionDescriptor, Func`2 modelFunction, IWebApiRequestMessage request, Func`2 createQueryOptionFunction)\r\n   at Microsoft.AspNet.OData.EnableQueryAttribute.OnActionExecuted(Object responseValue, IQueryable singleResultCollection, IWebApiActionDescriptor actionDescriptor, IWebApiRequestMessage request, Func`2 modelFunction, Func`2 createQueryOptionFunction, Action`1 createResponseAction, Action`3 createErrorAction)"
    }
  }
}

Press any key to continue...


```