// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace ODataWebV3.Northwind.Model
{
    #region Namespaces

    using System.Data.Services.Common;

    #endregion

    [DataServiceKey("CustomerID", "CustomerTypeID")]
    public partial class CustomerCustomerDemo
    {
    }

    [DataServiceKey("CustomerTypeID")]
    public partial class CustomerDemographic
    {
    }

    [DataServiceKey("EmployeeID", "TerritoryID")]
    public partial class Territory
    {
    }

    [DataServiceKey("OrderID", "ProductID")]
    public partial class Order_Detail
    {
    }

    [DataServiceKey("CustomerName", "Salesperson", "OrderID", "ShipperName", "ProductID", "ProductName", "UnitPrice", "Quantity", "Discount")]
    public partial class Invoice
    {
    }

    [DataServiceKey("ProductID", "ProductName", "Discontinued", "CategoryName")]
    public partial class Alphabetical_list_of_product
    {
    }

    [DataServiceKey("ProductID", "ProductName")]
    public partial class Current_Product_List
    {
    }

    [DataServiceKey("CategoryName")]
    public partial class Category_Sales_for_1997
    {
    }

    [DataServiceKey("CompanyName", "Relationship")]
    public partial class Customer_and_Suppliers_by_City
    {
    }

    [DataServiceKey("OrderID", "ProductID", "ProductName", "UnitPrice", "Quantity", "Discount", "ExtendedPrice")]
    public partial class Order_Details_Extended
    {
    }

    [DataServiceKey("OrderID")]
    public partial class Order_Subtotal
    {
    }

    [DataServiceKey("OrderID", "CompanyName")]
    public partial class Orders_Qry
    {
    }

    [DataServiceKey("CategoryName", "ProductName")]
    public partial class Product_Sales_for_1997
    {
    }

    [DataServiceKey("ProductName")]
    public partial class Products_Above_Average_Price
    {
    }

    [DataServiceKey("CategoryName", "ProductName", "Discontinued")]
    public partial class Products_by_Category
    {
    }

    [DataServiceKey("CategoryID", "CategoryName", "ProductName")]
    public partial class Sales_by_Category
    {
    }

    [DataServiceKey("OrderID", "CompanyName")]
    public partial class Sales_Totals_by_Amount
    {
    }

    [DataServiceKey("OrderID")]
    public partial class Summary_of_Sales_by_Quarter
    {
    }

    [DataServiceKey("OrderID")]
    public partial class Summary_of_Sales_by_Year
    {
    }
}