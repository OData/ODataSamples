# ODataEnumTypeSample
------------------

This sample implements a very simple employee management system, which can query, add employees and add access level to them.

It illustrates how to use Enum Type within an OData service. Such as:
1. Enum Type Properties
2. Enum Type is used in $filter
3. Enum Type is used as a function parameter type
4. Enum Type is used as a return type of an action

------------------

## .NET Classic

It's .NET Framework Console application self-hosting a Web API service depending on `Microsoft.AspNet.OData` nuget package.

When it runs, it does the a lot of requests, for example query metadata, filer employee using gender, etc.

Below is the console output:

```c#
Listening on http://localhost:12345

The metadata is
 <?xml version="1.0" encoding="utf-8"?><edmx:Edmx Version="4.0" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx"><edmx:DataServices><Schema Namespace="ODataEnumTypeSample" xmlns="http://docs.oasis-open.org/odata/ns/edm"><EntityType Name="Employee"><Key><PropertyRef Name="ID" /></Key><Property Name="ID" Type="Edm.Int32" Nullable="false" /><Property Name="Name" Type="Edm.String" /><Property Name="Gender" Type="ODataEnumTypeSample.Gender" Nullable="false" /><Property Name="AccessLevel" Type="ODataEnumTypeSample.AccessLevel" Nullable="false" /></EntityType><EnumType Name="AccessLevel" IsFlags="true"><Member Name="Read" Value="1" /><Member Name="Write" Value="2" /><Member Name="Execute" Value="4" /></EnumType><EnumType Name="Gender"><Member Name="Male" Value="1" /><Member Name="Female" Value="2" /></EnumType><Action Name="AddAccessLevel" IsBound="true"><Parameter Name="bindingParameter" Type="ODataEnumTypeSample.Employee" /><Parameter Name="AccessLevel" Type="Edm.String" Unicode="false" /><ReturnType Type="ODataEnumTypeSample.AccessLevel" Nullable="false" /></Action><Function Name="HasAccessLevel"><Parameter Name="ID" Type="Edm.Int32" Nullable="false" /><Parameter Name="AccessLevel" Type="ODataEnumTypeSample.AccessLevel" Nullable="false" /><ReturnType Type="Edm.Boolean" Nullable="false" /></Function><EntityContainer Name="Container"><EntitySet Name="Employees" EntityType="ODataEnumTypeSample.Employee" /><FunctionImport Name="HasAccessLevel" Function="ODataEnumTypeSample.HasAccessLevel" IncludeInServiceDocument="true" /></EntityContainer></Schema></edmx:DataServices></edmx:Edmx>

Employees whose gender is 'Male' are:
{
  "@odata.context": "http://localhost:12345/odata/$metadata#Employees",
  "value": [
    {
      "ID": 2,
      "Name": "Bob",
      "Gender": "Male",
      "AccessLevel": "Read"
    }
  ]
}

Employees who has the access level 'Read' are:
{
  "@odata.context": "http://localhost:12345/odata/$metadata#Employees",
  "value": [
    {
      "ID": 2,
      "Name": "Bob",
      "Gender": "Male",
      "AccessLevel": "Read"
    },
    {
      "ID": 3,
      "Name": "Alice",
      "Gender": "Female",
      "AccessLevel": "Read, Write"
    }
  ]
}

Employee with ID '1' has access level 'Execute':
{
  "@odata.context": "http://localhost:12345/odata/$metadata#Edm.Boolean",
  "value": true
}

The new access level of employee with ID '1' is:
{
  "@odata.context": "http://localhost:12345/odata/$metadata#ODataEnumTypeSample.AccessLevel",
  "value": "Read, Execute"
}

The newly added employee is:
{
  "@odata.context": "http://localhost:12345/odata/$metadata#Employees/$entity",
  "ID": 4,
  "Name": "Ben",
  "Gender": "Male",
  "AccessLevel": "Read, Write"
}

Press any key to continue...

```
