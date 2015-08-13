ODataAlternateKeySample
-------------------------

This sample illustrates how to use the Alternate key in Web API OData.

The sample implements three alternate key scenarios:

1. Single alternate keys
   for example:

   declared keys : ~/odata/Customers(3)
   alternate keys: ~/odata/Customer(SSN='SSN-3-103')

2. Multiple alternate keys
   for example:

   declared keys : ~/odata/Orders(2)
   alternate keys: ~/odata/Orders(Name='Order-2')
   alternate keys: ~/odata/Orders(Token=75036B94-C836-4946-8CC8-054CF54060EC)

3 Composition alternate keys
   for example:

   declared keys : ~/odata/People(2)
   alternate keys: ~/odata/Orders(Country='United States',Passport='9999')


This sample is provided as part of the OData WebApi sample repository at
https://github.com/OData/ODataSamples