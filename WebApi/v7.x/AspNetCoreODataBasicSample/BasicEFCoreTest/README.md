
# The Basic EF Core Sample        

---
This sample project is a test for EF Core with SQL Server.

## Query All

```C#
GET http://localhost:5000/odata/Customers
```

```json
{
    "@odata.context": "http://localhost:5000/odata/$metadata#Customers",
    "value": [
        {
            "Id": 1,
            "Name": "Customer A",
            "Age": 18,
            "FavoriateColor": "Yellow",
            "HomeAddress": {
                "City": "Redmond",
                "Street": "156 AVE NE"
            }
        },
        {
            "Id": 2,
            "Name": "Customer B",
            "Age": 19,
            "FavoriateColor": "Yellow",
            "HomeAddress": {
                "City": "Bellevue",
                "Street": "Main St NE"
            }
        }
    ]
}
```

The log shows:

```C#
info: Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager[0]
      User profile is available. Using 'C:\Users\saxu\AppData\Local\ASP.NET\DataProtection-Keys' as key repository and Windows DPAPI to encrypt keys at rest.
Hosting environment: Development
Content root path: E:\github\xuzhg\WebApiSample\AspNetCore\BasicEFCoreTest\BasicEFCoreTest
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
info: Microsoft.AspNetCore.Hosting.Internal.WebHost[1]
      Request starting HTTP/1.1 GET http://localhost:5000/api/values
info: Microsoft.AspNetCore.Hosting.Internal.WebHost[2]
      Request finished in 62.5218ms 404
info: Microsoft.AspNetCore.Hosting.Internal.WebHost[1]
      Request starting HTTP/1.1 GET http://localhost:5000/odata/Customers
info: Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker[1]
      Route matched with {action = "Get", controller = "Customers"}. Executing action BasicEFCoreTest.Controllers.CustomersController.Get (BasicEFCoreTest)
dbug: Microsoft.EntityFrameworkCore.Infrastructure[10401]
      An 'IServiceProvider' was created for internal use by Entity Framework.
dbug: Microsoft.EntityFrameworkCore.Model[10600]
      The property 'CustomerId' on entity type 'Address' was created in shadow state because there are no eligible CLR members with a matching name.
dbug: Microsoft.EntityFrameworkCore.Model[10600]
      The property 'OrderId' on entity type 'Customer' was created in shadow state because there are no eligible CLR members with a matching name.
warn: Microsoft.EntityFrameworkCore.Model.Validation[30000]
      No type was specified for the decimal column 'Price' on entity type 'Order'. This will cause values to be silently truncated if they do not fit in the default precision and scale. Explicitly specify the SQL server column type that can accommodate all the values using 'ForHasColumnType()'.
info: Microsoft.EntityFrameworkCore.Infrastructure[10403]
      Entity Framework Core 2.1.0-rtm-30799 initialized 'CustomerOrderContext' using provider 'Microsoft.EntityFrameworkCore.SqlServer' with options: None
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20000]
      Opening connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20001]
      Opened connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20002]
      Closing connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20003]
      Closed connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20000]
      Opening connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20001]
      Opened connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
dbug: Microsoft.EntityFrameworkCore.Database.Command[20100]
      Executing DbCommand [Parameters=[], CommandType='Text', CommandTimeout='30']
      IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE') SELECT 1 ELSE SELECT 0
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (27ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE') SELECT 1 ELSE SELECT 0
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20002]
      Closing connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20003]
      Closed connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
info: Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker[1]
      Executing action method BasicEFCoreTest.Controllers.CustomersController.Get (BasicEFCoreTest) - Validation state: Valid
info: Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker[2]
      Executed action method BasicEFCoreTest.Controllers.CustomersController.Get (BasicEFCoreTest), returned result Microsoft.AspNetCore.Mvc.OkObjectResult in 0.3325ms.
info: Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor[1]
      Executing ObjectResult, writing value of type 'Microsoft.EntityFrameworkCore.Internal.InternalDbSet`1[[BasicEFCoreTest.Models.Customer, BasicEFCoreTest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]'.
dbug: Microsoft.EntityFrameworkCore.Query[10101]
      => Microsoft.EntityFrameworkCore.Query.RelationalQueryModelVisitor
      Compiling query model:
      'from Customer <generated>_0 in DbSet<Customer>
      select [<generated>_0]'
dbug: Microsoft.EntityFrameworkCore.Query[10105]
      => Microsoft.EntityFrameworkCore.Query.RelationalQueryModelVisitor
      Including navigation: '[<generated>_0].'
dbug: Microsoft.EntityFrameworkCore.Query[10104]
      => Microsoft.EntityFrameworkCore.Query.RelationalQueryModelVisitor
      Optimized query model:
      'from Customer <generated>_0 in DbSet<Customer>
      join Address c.HomeAddress in DbSet<Address>
      on (Nullable<int>)Property([<generated>_0], "Id") equals Property([c.HomeAddress], "CustomerId") into c.HomeAddress_group
      from Address c.HomeAddress in
          (from Address c.HomeAddress_groupItem in [c.HomeAddress_group]
          select [c.HomeAddress_groupItem]).DefaultIfEmpty()
      select Customer _Include(
          queryContext: queryContext,
          entity: [<generated>_0],
          included: new object[]{ [c.HomeAddress] },
          fixup: (QueryContext queryContext | Customer entity | object[] included) =>
          {
              void queryContext.QueryBuffer.StartTracking(
                  entity: entity,
                  entityType: EntityType: Customer)
              return !(bool ReferenceEquals(included[0], null)) ?
              {
                  void queryContext.QueryBuffer.StartTracking(
                      entity: included[0],
                      entityType: EntityType: Address)
                  return void SetRelationshipSnapshotValue(
                      stateManager: queryContext.StateManager,
                      navigation: Customer.HomeAddress,
                      entity: entity,
                      value: included[0])
              }
               :
              {
                  void SetRelationshipIsLoaded(
                      stateManager: queryContext.StateManager,
                      navigation: Customer.HomeAddress,
                      entity: entity)
                  return default(void)
              }

          }
      )'
dbug: Microsoft.EntityFrameworkCore.Query[10107]
      => Microsoft.EntityFrameworkCore.Query.RelationalQueryModelVisitor
      (QueryContext queryContext) => IEnumerable<Customer> _InterceptExceptions(
          source: IEnumerable<Customer> _TrackEntities(
              results: IEnumerable<Customer> _Select(
                  source: IEnumerable<TransparentIdentifier<Customer, Address>> _ShapedQuery(
                      queryContext: queryContext,
                      shaperCommandContext: SelectExpression:
                          SELECT [c].[Id], [c].[Age], [c].[FavoriateColor], [c].[Name], [c].[OrderId], [c].[Id], [c].[HomeAddress_City], [c].[HomeAddress_Street]
                          FROM [Customers] AS [c],
                      shaper: TypedCompositeShaper<BufferedOffsetEntityShaper<Customer>, Customer, BufferedOffsetEntityShaper<Address>, Address, TransparentIdentifier<Customer, Address>>),
                  selector: (TransparentIdentifier<Customer, Address> t1) => Customer _Include(
                      queryContext: queryContext,
                      entity: t1.Outer,
                      included: new object[]{ t1.Inner },
                      fixup: (QueryContext queryContext | Customer entity | object[] included) =>
                      {
                          void queryContext.QueryBuffer.StartTracking(
                              entity: entity,
                              entityType: EntityType: Customer)
                          return !(bool ReferenceEquals(included[0], null)) ?
                          {
                              void queryContext.QueryBuffer.StartTracking(
                                  entity: included[0],
                                  entityType: EntityType: Address)
                              return void SetRelationshipSnapshotValue(
                                  stateManager: queryContext.StateManager,
                                  navigation: Customer.HomeAddress,
                                  entity: entity,
                                  value: included[0])
                          }
                           :
                          {
                              void SetRelationshipIsLoaded(
                                  stateManager: queryContext.StateManager,
                                  navigation: Customer.HomeAddress,
                                  entity: entity)
                              return default(void)
                          }
                      }
                  )),
              queryContext: Unhandled parameter: queryContext,
              entityTrackingInfos: { itemType: Customer },
              entityAccessors: List<Func<Customer, object>>
              {
                  Func<Customer, Customer>,
              }
          ),
          contextType: BasicEFCoreTest.Models.CustomerOrderContext,
          logger: DiagnosticsLogger<Query>,
          queryContext: Unhandled parameter: queryContext)
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20000]
      Opening connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20001]
      Opened connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
dbug: Microsoft.EntityFrameworkCore.Database.Command[20100]
      Executing DbCommand [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT [c].[Id], [c].[Age], [c].[FavoriateColor], [c].[Name], [c].[OrderId], [c].[Id], [c].[HomeAddress_City], [c].[HomeAddress_Street]
      FROM [Customers] AS [c]
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (28ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT [c].[Id], [c].[Age], [c].[FavoriateColor], [c].[Name], [c].[OrderId], [c].[Id], [c].[HomeAddress_City], [c].[HomeAddress_Street]
      FROM [Customers] AS [c]
dbug: Microsoft.EntityFrameworkCore.ChangeTracking[10806]
      Context 'Customer' started tracking 'CustomerOrderContext' entity. Consider using 'DbContextOptionsBuilder.EnableSensitiveDataLogging' to see key values.
dbug: Microsoft.EntityFrameworkCore.ChangeTracking[10806]
      Context 'Address' started tracking 'CustomerOrderContext' entity. Consider using 'DbContextOptionsBuilder.EnableSensitiveDataLogging' to see key values.
dbug: Microsoft.EntityFrameworkCore.ChangeTracking[10806]
      Context 'Customer' started tracking 'CustomerOrderContext' entity. Consider using 'DbContextOptionsBuilder.EnableSensitiveDataLogging' to see key values.
dbug: Microsoft.EntityFrameworkCore.ChangeTracking[10806]
      Context 'Address' started tracking 'CustomerOrderContext' entity. Consider using 'DbContextOptionsBuilder.EnableSensitiveDataLogging' to see key values.
dbug: Microsoft.EntityFrameworkCore.Database.Command[20300]
      A data reader was disposed.
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20002]
      Closing connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20003]
      Closed connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
info: Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker[2]
      Executed action BasicEFCoreTest.Controllers.CustomersController.Get (BasicEFCoreTest) in 929.6248ms
info: Microsoft.AspNetCore.Hosting.Internal.WebHost[2]
      Request finished in 1022.7123ms 200 application/json; odata.metadata=minimal; odata.streaming=true; charset=utf-8
dbug: Microsoft.EntityFrameworkCore.Infrastructure[10407]
      'CustomerOrderContext' disposed.
```


## Query certain property

```C#
Get http://localhost:5000/odata/Customers?$select=Age&$top=1
```

```json
{
    "@odata.context": "http://localhost:5000/odata/$metadata#Customers(Age)",
    "value": [
        {
            "Age": 18
        }
    ]
}

```

The log shows:

```C#
info: Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager[0]
      User profile is available. Using 'C:\Users\saxu\AppData\Local\ASP.NET\DataProtection-Keys' as key repository and Windows DPAPI to encrypt keys at rest.
Hosting environment: Development
Content root path: E:\github\xuzhg\WebApiSample\AspNetCore\BasicEFCoreTest\BasicEFCoreTest
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
info: Microsoft.AspNetCore.Hosting.Internal.WebHost[1]
      Request starting HTTP/1.1 GET http://localhost:5000/api/values
info: Microsoft.AspNetCore.Hosting.Internal.WebHost[2]
      Request finished in 75.4887ms 404
info: Microsoft.AspNetCore.Hosting.Internal.WebHost[1]
      Request starting HTTP/1.1 GET http://localhost:5000/odata/Customers?$select=Age&$top=1
info: Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker[1]
      Route matched with {action = "Get", controller = "Customers"}. Executing action BasicEFCoreTest.Controllers.CustomersController.Get (BasicEFCoreTest)
dbug: Microsoft.EntityFrameworkCore.Infrastructure[10401]
      An 'IServiceProvider' was created for internal use by Entity Framework.
dbug: Microsoft.EntityFrameworkCore.Model[10600]
      The property 'CustomerId' on entity type 'Address' was created in shadow state because there are no eligible CLR members with a matching name.
dbug: Microsoft.EntityFrameworkCore.Model[10600]
      The property 'OrderId' on entity type 'Customer' was created in shadow state because there are no eligible CLR members with a matching name.
warn: Microsoft.EntityFrameworkCore.Model.Validation[30000]
      No type was specified for the decimal column 'Price' on entity type 'Order'. This will cause values to be silently truncated if they do not fit in the default precision and scale. Explicitly specify the SQL server column type that can accommodate all the values using 'ForHasColumnType()'.
info: Microsoft.EntityFrameworkCore.Infrastructure[10403]
      Entity Framework Core 2.1.0-rtm-30799 initialized 'CustomerOrderContext' using provider 'Microsoft.EntityFrameworkCore.SqlServer' with options: None
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20000]
      Opening connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20001]
      Opened connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20002]
      Closing connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20003]
      Closed connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20000]
      Opening connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20001]
      Opened connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
dbug: Microsoft.EntityFrameworkCore.Database.Command[20100]
      Executing DbCommand [Parameters=[], CommandType='Text', CommandTimeout='30']
      IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE') SELECT 1 ELSE SELECT 0
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (28ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE') SELECT 1 ELSE SELECT 0
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20002]
      Closing connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20003]
      Closed connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
info: Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker[1]
      Executing action method BasicEFCoreTest.Controllers.CustomersController.Get (BasicEFCoreTest) - Validation state: Valid
info: Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker[2]
      Executed action method BasicEFCoreTest.Controllers.CustomersController.Get (BasicEFCoreTest), returned result Microsoft.AspNetCore.Mvc.OkObjectResult in 0.3074ms.
info: Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor[1]
      Executing ObjectResult, writing value of type 'Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable`1[[Microsoft.AspNet.OData.Query.Expressions.SelectExpandBinder+SelectSome`1[[BasicEFCoreTest.Models.Customer, BasicEFCoreTest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], Microsoft.AspNetCore.OData, Version=7.0.0.20608, Culture=neutral, PublicKeyToken=31bf3856ad364e35]]'.
dbug: Microsoft.EntityFrameworkCore.Query[10101]
      => Microsoft.EntityFrameworkCore.Query.RelationalQueryModelVisitor
      Compiling query model:
      'from Customer <generated>_1 in
          (from Customer $it in DbSet<Customer>
          order by [$it].Id asc
          select [$it]).Take(__TypedProperty_0)
      select new SelectSome<Customer>{
          ModelID = "311bb535-d197-44ac-93e6-05e5dc20da45",
          Container = new NamedPropertyWithNext0<Nullable<int>>{
              Name = "Age",
              Value = (Nullable<int>)[<generated>_1].Age,
              Next0 = new AutoSelectedNamedProperty<Nullable<int>>{
                  Name = "Id",
                  Value = (Nullable<int>)[<generated>_1].Id
              }

          }

      }
      '
dbug: Microsoft.EntityFrameworkCore.Query[10104]
      => Microsoft.EntityFrameworkCore.Query.RelationalQueryModelVisitor
      Optimized query model:
      '(from Customer $it in DbSet<Customer>
      order by [$it].Id asc
      select new SelectSome<Customer>{
          ModelID = "311bb535-d197-44ac-93e6-05e5dc20da45",
          Container = new NamedPropertyWithNext0<Nullable<int>>{
              Name = "Age",
              Value = (Nullable<int>)[$it].Age,
              Next0 = new AutoSelectedNamedProperty<Nullable<int>>{
                  Name = "Id",
                  Value = (Nullable<int>)[$it].Id
              }

          }

      }
      ).Take(__TypedProperty_0)'
dbug: Microsoft.EntityFrameworkCore.Query[10107]
      => Microsoft.EntityFrameworkCore.Query.RelationalQueryModelVisitor
      (QueryContext queryContext) => IEnumerable<SelectSome<Customer>> _InterceptExceptions(
          source: IEnumerable<SelectSome<Customer>> _ShapedQuery(
              queryContext: queryContext,
              shaperCommandContext: SelectExpression:
                  SELECT TOP(@__TypedProperty_0) [$it].[Age] AS [Value0], [$it].[Id] AS [Value]
                  FROM [Customers] AS [$it]
                  ORDER BY [Value],
              shaper: TypedProjectionShaper<ValueBufferShaper, ValueBuffer, SelectSome<Customer>>),
          contextType: BasicEFCoreTest.Models.CustomerOrderContext,
          logger: DiagnosticsLogger<Query>,
          queryContext: queryContext)
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20000]
      Opening connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20001]
      Opened connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
dbug: Microsoft.EntityFrameworkCore.Database.Command[20100]
      Executing DbCommand [Parameters=[@__TypedProperty_0='?' (DbType = Int32)], CommandType='Text', CommandTimeout='30']
      SELECT TOP(@__TypedProperty_0) [$it].[Age] AS [Value0], [$it].[Id] AS [Value]
      FROM [Customers] AS [$it]
      ORDER BY [Value]
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (4ms) [Parameters=[@__TypedProperty_0='?' (DbType = Int32)], CommandType='Text', CommandTimeout='30']
      SELECT TOP(@__TypedProperty_0) [$it].[Age] AS [Value0], [$it].[Id] AS [Value]
      FROM [Customers] AS [$it]
      ORDER BY [Value]
dbug: Microsoft.EntityFrameworkCore.Database.Command[20300]
      A data reader was disposed.
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20002]
      Closing connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
dbug: Microsoft.EntityFrameworkCore.Database.Connection[20003]
      Closed connection to database 'Demo.CustomerOrderSam' on server '(localdb)\mssqllocaldb'.
info: Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker[2]
      Executed action BasicEFCoreTest.Controllers.CustomersController.Get (BasicEFCoreTest) in 929.6644ms
dbug: Microsoft.EntityFrameworkCore.Infrastructure[10407]
      'CustomerOrderContext' disposed.
info: Microsoft.AspNetCore.Hosting.Internal.WebHost[2]
      Request finished in 1016.1331ms 200 application/json; odata.metadata=minimal; odata.streaming=true; charset=utf-8
```

## Filter on certain property

If you issue a request as:
```C#
http://localhost:5000/odata/Customers?$filter=contains(UserName, 'Pe')
```

you will get the payload as:

```json
{
    "@odata.context": "http://localhost:5000/odata/$metadata#Customers",
    "value": [
        {
            "Id": 2,
            "FirstName": "Jonier",
            "LastName": "Alice",
            "UserName": "Peter",
            "Age": 19,
            "FavoriateColor": "Red",
            "HomeAddress": {
                "City": "Bellevue",
                "Street": "Main St NE"
            }
        }
    ]
}
```
