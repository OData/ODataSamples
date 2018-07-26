# ODataOpenTypeSample
------------------

Entity or complex types which allow clients to persist additional undeclared properties are called open types. The additional undeclared properties are called dynamic properties.

This sample illustrates:

1. Build the edm model with Open types (Entity type and Complex type)
2. Add/update an open entity object with open a complex type property.
3. CRUD open entities.


------------------

## .NET Classic

It's .NET Framework Console application self-hosting a Web API service depending on `Microsoft.AspNet.OData` nuget package.

When it runs, it does the following requests:

1. GET ~/$metadata
2. GET ~/Accounts(1)
3. GET ~/Accounts(1)/Address
4. POST ~/Accounts
5. PUT ~/Accounts(1)
6. PATCH ~/Accounts(1)

Below is the console output:

```c#
Listening on http://SAM-XU-REDMOND:12345
GET ~/$metadata
StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  OData-Version: 4.0
  Date: Fri, 20 Jul 2018 20:33:44 GMT
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 884
  Content-Type: application/xml
}
<?xml version="1.0" encoding="utf-8"?><edmx:Edmx Version="4.0" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx"><edmx:DataServices><Schema Namespace="ODataOpenTypeSample" xmlns="http://docs.oasis-open.org/odata/ns/edm"><EntityType Name="Account" OpenType="true"><Key><PropertyRef Name="Id" /></Key><Property Name="Id" Type="Edm.Int32" Nullable="false" /><Property Name="Name" Type="Edm.String" /><Property Name="Address" Type="ODataOpenTypeSample.Address" /></EntityType><ComplexType Name="Address" OpenType="true"><Property Name="City" Type="Edm.String" /><Property Name="Street" Type="Edm.String" /></ComplexType><EnumType Name="Gender"><Member Name="Male" Value="1" /><Member Name="Female" Value="2" /></EnumType><EntityContainer Name="Container"><EntitySet Name="Accounts" EntityType="ODataOpenTypeSample.Account" /></EntityContainer></Schema></edmx:DataServices></edmx:Edmx>
GET ~/Accounts(1)
StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  OData-Version: 4.0
  Date: Fri, 20 Jul 2018 20:33:44 GMT
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 313
  Content-Type: application/json; odata.metadata=minimal
}
{
  "@odata.context": "http://sam-xu-redmond:12345/odata/$metadata#Accounts/$entity",
  "Id": 1,
  "Name": "Name1",
  "Gender@odata.type": "#ODataOpenTypeSample.Gender",
  "Gender": "Male",
  "Emails@odata.type": "#Collection(String)",
  "Emails": [
    "a@a.com",
    "b@b.com"
  ],
  "Address": {
    "City": "Redmond",
    "Street": "1 Microsoft Way",
    "Country": "US"
  }
}
GET ~/Accounts(1)/Address
StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  OData-Version: 4.0
  Date: Fri, 20 Jul 2018 20:33:44 GMT
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 143
  Content-Type: application/json; odata.metadata=minimal
}
{
  "@odata.context": "http://sam-xu-redmond:12345/odata/$metadata#Accounts(1)/Address",
  "City": "Redmond",
  "Street": "1 Microsoft Way",
  "Country": "US"
}
POST ~/Accounts
StatusCode: 201, ReasonPhrase: 'Created', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  OData-Version: 4.0
  Date: Fri, 20 Jul 2018 20:33:44 GMT
  Location: http://sam-xu-redmond:12345/odata/Accounts(2)
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 308
  Content-Type: application/json; odata.metadata=minimal
}
{
  "@odata.context": "http://sam-xu-redmond:12345/odata/$metadata#Accounts/$entity",
  "Id": 2,
  "Name": "Ben",
  "Gender@odata.type": "#ODataOpenTypeSample.Gender",
  "Gender": "Female",
  "Emails@odata.type": "#Collection(String)",
  "Emails": [
    "a@a.com",
    "b@b.com"
  ],
  "Address": {
    "City": "Shanghai",
    "Street": "Zixing",
    "Country": "China"
  }
}
PUT ~/Accounts(1)
StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  OData-Version: 4.0
  Date: Fri, 20 Jul 2018 20:33:44 GMT
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 240
  Content-Type: application/json; odata.metadata=minimal
}
{
  "@odata.context": "http://sam-xu-redmond:12345/odata/$metadata#Accounts/$entity",
  "Id": 1,
  "Name": "Jinfu",
  "Emails@odata.type": "#Collection(String)",
  "Emails": [
    "d@d.com",
    "e@e.com"
  ],
  "Address": {
    "City": "Beijing",
    "Street": "Changan",
    "Zip": "200-099"
  }
}
PATCH ~/Accounts(1)
StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
{
  OData-Version: 4.0
  Date: Fri, 20 Jul 2018 20:33:44 GMT
  Server: Microsoft-HTTPAPI/2.0
  Content-Length: 300
  Content-Type: application/json; odata.metadata=minimal
}
{
  "@odata.context": "http://sam-xu-redmond:12345/odata/$metadata#Accounts/$entity",
  "Id": 1,
  "Name": "Jinfu",
  "Emails@odata.type": "#Collection(String)",
  "Emails": [],
  "Whatever@odata.type": "#Collection(Int32)",
  "Whatever": [
    1,
    2,
    3
  ],
  "Address": {
    "City": "Beijing",
    "Street": "Changan",
    "Zip": "200-099",
    "IsDefault": true
  }
}

Press any key to continue...
```
```