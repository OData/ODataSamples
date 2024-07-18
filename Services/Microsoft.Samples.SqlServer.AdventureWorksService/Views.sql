-- Copyright Microsoft

-- Views for AdventureWorks2012 OData Service
USE AdventureWorks2012
GO

PRINT 'Create Views for AdventureWorks2012 OData Service'
GO

PRINT 'Create [Production].[vProductCatalog] View'
IF EXISTS (SELECT [name] FROM [sys].[views] WHERE [name] = N'vProductCatalog')
  DROP VIEW [Production].vProductCatalog
GO
CREATE VIEW [Production].vProductCatalog
AS
SELECT ROW_NUMBER() OVER (ORDER BY [ProductID] DESC) AS ID, P.ProductID, P.ProductNumber, P.Name AS ProductName, PM.Name AS ProductModel, PC.Name AS ProductCategory,
                  PS.Name AS ProductSubcategory, PD.Description, PMPDCL.CultureID, P.Color, P.Size, P.Weight, P.ListPrice
FROM Production. Product AS P INNER JOIN
                  Production.ProductSubcategory AS PS INNER JOIN
				  Production.ProductCategory AS PC ON PS.ProductCategoryID = PC.ProductCategoryID ON P.ProductSubcategoryID = PS.ProductSubcategoryID INNER JOIN
				  Production.ProductDescription AS PD INNER JOIN
				  Production.ProductModel AS PM INNER JOIN
				  Production.ProductModelProductDescriptionCulture AS PMPDCL ON PM.ProductModelID = PMPDCL.ProductModelID ON
				  PD.ProductDescriptionID = PMPDCL.ProductDescriptionID ON P.ProductModelID = PM.ProductModelID
GO

PRINT 'Create [Production].[vManufacturingInstructions] View'
IF EXISTS (SELECT [name] FROM [sys].[views] WHERE [name] = N'vManufacturingInstructions')
  DROP VIEW [Production].[vManufacturingInstructions]
GO

CREATE VIEW [Production].[vManufacturingInstructions]
AS
SELECT ROW_NUMBER() OVER(ORDER BY [ProductModelID] DESC) AS ID, [ProductModelID], [Name] as [ProductName],
                  [Instructions].value(N'declare default element namespace "http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ProductModelManuInstructions"; (/root/text())[1]', 'nvarchar(max)') AS [Instructions],
                  [MfgInstructions].ref.value('@LocationID[1]', 'int') AS [LocationID],
                  [MfgInstructions].ref.value('@SetupHours[1]', 'decimal(9, 4)') AS [SetupHours],
                  [MfgInstructions].ref.value('@MachineHours[1]', 'decimal(9, 4)') AS [MachineHours],
                  [MfgInstructions].ref.value('@LaborHours[1]', 'decimal(9, 4)') AS [LaborHours],
                  [MfgInstructions].ref.value('@LotSize[1]', 'int') AS [LotSize],
                  [Steps].ref.value('string(.)[1]', 'nvarchar(1024)') AS [Step]
FROM [Production].[ProductModel] CROSS APPLY
                  [Instructions].nodes(N'declare default element namespace "http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ProductModelManuInstructions"; /root/Location') MfgInstructions(ref) CROSS APPLY
				  [MfgInstructions].ref.nodes('declare default element namespace "http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ProductModelManuInstructions"; step') Steps(ref)
GO

PRINT 'Create [Sales].[vCompanySales] View'
IF EXISTS (SELECT [name] FROM [sys].[views] WHERE [name] = N'vCompanySales')
  DROP VIEW [Sales].[vCompanySales]
GO
CREATE VIEW [Sales].[vCompanySales]
AS
SELECT ROW_NUMBER() OVER(ORDER BY PC.Name DESC) AS ID, PC.Name AS ProductCategory, PS.Name AS ProductSubCategory, DATEPART(yy, SOH.OrderDate) AS OrderYear,
                  'Q' + DATENAME(qq, SOH.OrderDate) AS OrderQtr, SUM(SOD.UnitPrice * SOD.OrderQty) AS Sales
FROM Production.ProductSubcategory AS PS INNER JOIN
                  Sales.SalesOrderHeader AS SOH INNER JOIN
				  Sales.SalesOrderDetail AS SOD ON SOH.SalesOrderID = SOD.SalesOrderID INNER JOIN
				  Production.Product AS P ON SOD.ProductID = P.ProductID ON PS.ProductSubcategoryID = P.ProductSubcategoryID INNER JOIN
				  Production.ProductCategory AS PC ON PS.ProductCategoryID = PC.ProductCategoryID
WHERE (SOH.OrderDate BETWEEN '1/1/2002' AND '12/31/2015')
GROUP BY DATEPART(yy, SOH.OrderDate), PC.Name, PS.Name, 'Q' + DATENAME(qq, SOH.OrderDate), PS.ProductSubcategoryID
GO

PRINT 'Create [Sales].[vTerritorySalesDrilldown] View'
IF EXISTS (SELECT [name] FROM [sys].[views] WHERE [name] = N'vTerritorySalesDrilldown')
  DROP VIEW [Sales].[vTerritorySalesDrilldown]
GO
CREATE VIEW Sales.vTerritorySalesDrilldown
AS
SELECT ROW_NUMBER() OVER(ORDER BY SP.BusinessEntityID DESC) AS ID, ST.Name as [TerritoryName], SP.BusinessEntityID AS [SalesPersonID],
                  C.FirstName as [EmployeeFirstName], C.LastName as [EmployeeLastName], SOH.SalesOrderNumber, SOH.TotalDue as [Total]
FROM Sales.SalesTerritory AS ST INNER JOIN
                  Sales.SalesPerson AS SP ON ST.TerritoryID = SP.TerritoryID INNER JOIN
				  HumanResources.Employee AS E ON SP.BusinessEntityID = E.BusinessEntityID INNER JOIN
				  Person.Person AS C ON E.BusinessEntityID = C.BusinessEntityID INNER JOIN
				  Sales.SalesOrderHeader AS SOH ON SP.BusinessEntityID = SOH.SalesPersonID
GROUP BY ST.Name, SP.BusinessEntityID, C.FirstName, C.LastName, SOH.SalesOrderNumber, SOH.TotalDue
GO

PRINT 'Create [Production.[vWorkOrderRouting] View'
IF EXISTS (SELECT [name] FROM [sys].[views] WHERE [name] = N'vWorkOrderRouting')
  DROP VIEW [Production].[vWorkOrderRouting]
GO
CREATE VIEW [Production].[vWorkOrderRouting]
AS
SELECT ROW_NUMBER() OVER(ORDER BY Production.WorkOrderRouting.ProductID DESC) AS ID, Production.WorkOrderRouting.WorkOrderID AS [WorkOrderID],
                  Production.WorkOrderRouting.ProductID AS [ProductID], Production.Product.ProductNumber, Production.Product.Name AS [ProductName],
				  Production.WorkOrderRouting.OperationSequence AS [OperationSequence], Production.Location.LocationID, Production.Location.Name,
				  Production.WorkOrderRouting.ScheduledStartDate, Production.WorkOrderRouting.ActualStartDate, Production.WorkOrderRouting.ScheduledEndDate,
				  Production.WorkOrderRouting.ActualEndDate, Production.WorkOrderRouting.ActualResourceHrs, Production.WorkOrderRouting.PlannedCost,
				  Production.WorkOrderRouting.ActualCost, Production.WorkOrder.OrderQty, Production.WorkOrder.ScrappedQty, Production.WorkOrder.DueDate,
				  Production.WorkOrder.ScrapReasonID, Production.ScrapReason.Name AS [ScrapReason]
FROM Production.WorkOrderRouting INNER JOIN
                  Production.Location ON Production.WorkOrderRouting.LocationID = Production.Location.LocationID INNER JOIN
				  Production.Product ON Production.WorkOrderRouting.ProductID = Production.Product.ProductID INNER JOIN
				  Production.WorkOrder ON Production.WorkOrderRouting.WorkOrderID = Production.WorkOrder.WorkOrderID LEFT OUTER JOIN
				  Production.ScrapReason ON Production.WorkOrder.ScrapReasonID = Production.ScrapReason.ScrapReasonID
GO

PRINT 'Create [Production.[vDocument] View'
IF EXISTS (SELECT [name] FROM [sys].[views] WHERE [name] = N'vDocument')
  DROP VIEW [Production].[vDocument]
GO
CREATE VIEW [Production].[vDocument]
AS
SELECT DocumentNode.ToString() AS DocumentPath, DocumentLevel, Title, DocumentSummary, [Document], rowguid
FROM [Production].[Document]
GO