# ODataServiceSample
------------------

This sample illustrates how to create an OData service consisting of four OData Controllers.
The controllers provide various levels of functionality in terms of the OData functionality 
they expose. In addition the OData service exposes a $metadata document which allows the data 
to the consumed by OData Client for .NET clients and other clients (for example: Simiple.OData.Client) 
that accept the $metadata format.

* Supplier
    -> Collection<ProductFamily>
    -> Address

* ProductFamily
    -> Supplier
    -> Collection<Product>

* Product
    -> ProductFamily

In addition, Supplier has a complex type property called Address.

The sample contains three sample controllers demonstrating various levels of 
exposing OData:

* The SupplierController exposes a subset of functionality including Query, Get by Key and 
Create, by handling these requests:

  GET /Suppliers
  GET /Suppliers(key)
  GET /Suppliers?$filter=..&$orderby=..&$top=..&$skip=..
  POST /Suppliers

* The controllers leverages the ODataController base class which
exposes a useful pattern for implementing a rich OData service.

Furthermore, the ODataService exposes a Service Document (aka. $metadata document) that 
lists all the top-level entities so clients can discover them. This enables OData clients
to discover and consume OData Services exposed through ASP.NET Web API.

------------------
## Usage

1. Run ODataService.Web, it's an ASP.NET Core OData service.
2. Run ODataSerivce.SimpleODataClient or
3. Run ODataSerivce.Client


## Run ODataSerivce.SimpleODataClient.

It's an .NET Core console Application to run some OData service functionalities:

If you run it after running the ODataService.Web, You will get the following result:

```cmd
Welcome to the OData Web Api command line client sample.
        Type '?' for options.
> ?
Available commands:
        get products                   -> Print all the Products.
        query products                 -> Query Products.
        get productfamilies            -> Print all the ProductFamilies.
        ?                              -> Print Available Commands.
        quit                           -> Quit.
> query products

        << query products >>

        Get top 4 products
{
        Id: 1
        Name: MS-DOS 1.12 (OEM)
        ReleaseDate:
        SupportedUntil:
}
{
        Id: 2
        Name: Windows 98
        ReleaseDate: 6/25/1998 12:00:00 AM -07:00
        SupportedUntil: 7/11/2006 12:00:00 AM -07:00
}
{
        Id: 3
        Name: Windows Me
        ReleaseDate: 9/14/2000 12:00:00 AM -07:00
        SupportedUntil: 7/11/2006 12:00:00 AM -07:00
}
{
        Id: 4
        Name: Windows NT 3.1
        ReleaseDate: 7/27/1993 12:00:00 AM -07:00
        SupportedUntil: 12/31/2001 12:00:00 AM -08:00
}

        Get products with name starting with 'Microsoft Office'
{
        Id: 22
        Name: Microsoft Office 4.0
        ReleaseDate:
        SupportedUntil:
}
{
        Id: 23
        Name: Microsoft Office 4.2
        ReleaseDate:
        SupportedUntil:
}
{
        Id: 24
        Name: Microsoft Office 4.3
        ReleaseDate:
        SupportedUntil:
}
{
        Id: 25
        Name: Microsoft Office 95
        ReleaseDate:
        SupportedUntil:
}
{
        Id: 26
        Name: Microsoft Office 97
        ReleaseDate:
        SupportedUntil:
}
{
        Id: 27
        Name: Microsoft Office 2000
        ReleaseDate:
        SupportedUntil:
}
{
        Id: 28
        Name: Microsoft Office XP
        ReleaseDate:
        SupportedUntil:
}
{
        Id: 29
        Name: Microsoft Office 2003
        ReleaseDate:
        SupportedUntil:
}
{
        Id: 30
        Name: Microsoft Office 2007
        ReleaseDate:
        SupportedUntil:
}
{
        Id: 49
        Name: Microsoft Office 2010
        ReleaseDate:
        SupportedUntil:
}
{
        Id: 66
        Name: Microsoft Office 2013
        ReleaseDate:
        SupportedUntil:
}
```

## Run ODataSerivce.Client

This client has some running problems.