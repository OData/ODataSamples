// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace ODataWebV3.OData
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services;
    using System.Data.Services.Common;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Spatial;
    using System.Text;
    using System.Web;
    using DataServiceProviderV3;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.Edm.Library.Annotations;
    using Microsoft.Data.Edm.Library.Values;

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class ODataService : DSPDataService<DSPContext>
    {
        /// <summary>
        /// Maximum number of entities one can add to a container
        /// </summary>
        public const int MaxEntityPerContainer = 50;

        /// <summary>
        /// Maximum length of any string property
        /// </summary>
        public const int MaxStringPropertyLength = 256;

        /// <summary>
        /// Maximum number of items cached per session
        /// </summary>
        public const int MaxCachedSessionItems = 5000;

        /// <summary>
        /// The Session-Data cache
        /// </summary>
        private static Dictionary<String, DSPContext> _dsCache = new Dictionary<string, DSPContext>(MaxCachedSessionItems);

        /// <summary>
        /// The static metadata object
        /// </summary>
        private static DSPMetadata _metadata;

        /// <summary>
        /// The action provider
        /// </summary>
        private static DSPActionProvider _actionProvider;

        ///<summary>
        /// The stream provider
        /// </summary>
        private static InMemoryStreamProvider<ReferenceEqualityComparer> _streamProvider = new InMemoryStreamProvider<ReferenceEqualityComparer>();

        /// <summary>
        /// Spatial operations implementation
        /// </summary>
        private static SpatialOperationsImplementation _spatialOperations = new SpatialOperationsImplementation();

        #region Metadata and Data Generation

        /// <summary>
        /// Generates the default metadata for the service
        /// </summary>
        private static void CreateDefaultMetadata()
        {
            _metadata = new DSPMetadata("DemoService", "ODataDemo");

            #region Complex Type
            // Address Complex Type
            ResourceType address = _metadata.AddComplexType("Address");
            _metadata.AddPrimitiveProperty(address, "Street", typeof(string));
            _metadata.AddPrimitiveProperty(address, "City", typeof(string));
            _metadata.AddPrimitiveProperty(address, "State", typeof(string));
            _metadata.AddPrimitiveProperty(address, "ZipCode", typeof(string));
            _metadata.AddPrimitiveProperty(address, "Country", typeof(string));
            #endregion

            #region Entity type
            // Product Entity Type
            ResourceType product = _metadata.AddEntityType("Product");
            _metadata.AddKeyProperty(product, "ID", typeof(Int32));
            _metadata.AddPrimitiveProperty(product, "Name", typeof(string));
            _metadata.AddPrimitiveProperty(product, "Description", typeof(string));
            _metadata.AddPrimitiveProperty(product, "ReleaseDate", typeof(DateTime));
            _metadata.AddPrimitiveProperty(product, "DiscontinuedDate", typeof(DateTime?));
            _metadata.AddPrimitiveProperty(product, "Rating", typeof(Int16));
            _metadata.AddPrimitiveProperty(product, "Price", typeof(double));

            //ProductDetail Entity Type
            ResourceType productDetail = _metadata.AddEntityType("ProductDetail");
            _metadata.AddKeyProperty(productDetail, "ProductID", typeof(Int32));
            _metadata.AddPrimitiveProperty(productDetail, "Details", typeof(string));

            // Category Entity Type
            ResourceType category = _metadata.AddEntityType("Category", null, /* is OpenType */true);
            _metadata.AddKeyProperty(category, "ID", typeof(int));
            _metadata.AddPrimitiveProperty(category, "Name", typeof(string));

            // Supplier Entity Type
            ResourceType supplier = _metadata.AddEntityType("Supplier");
            _metadata.AddKeyProperty(supplier, "ID", typeof(int));
            _metadata.AddPrimitiveProperty(supplier, "Name", typeof(string));
            _metadata.AddComplexProperty(supplier, "Address", address);
            _metadata.AddPrimitiveProperty(supplier, "Location", typeof(GeographyPoint));
            _metadata.AddETagProperty(supplier, "Concurrency", typeof(int));

            // Person Entity Type
            ResourceType person = _metadata.AddEntityType("Person");
            _metadata.AddKeyProperty(person, "ID", typeof(int));
            _metadata.AddPrimitiveProperty(person, "Name", typeof(string));

            // PersonDetail Entity Type
            ResourceType personDetail = _metadata.AddEntityType("PersonDetail");
            _metadata.AddKeyProperty(personDetail, "PersonID", typeof(Int32));
            _metadata.AddPrimitiveProperty(personDetail, "Age", typeof(Byte));
            _metadata.AddPrimitiveProperty(personDetail, "Gender", typeof(Boolean));
            _metadata.AddPrimitiveProperty(personDetail, "Phone", typeof(string));
            _metadata.AddComplexProperty(personDetail, "Address", address);
            _metadata.AddStreamProperty(personDetail, "Photo");

            // Advertisement Entity Type
            ResourceType advertisement = _metadata.AddEntityType("Advertisement", null, /* isOpenType */false, /* isMediaLinkEntry */true);
            _metadata.AddKeyProperty(advertisement, "ID", typeof(Guid));
            _metadata.AddPrimitiveProperty(advertisement, "Name", typeof(string));
            _metadata.AddPrimitiveProperty(advertisement, "AirDate", typeof(DateTime));
            #endregion

            #region Entity Set
            ResourceSet products = _metadata.AddResourceSet("Products", product);
            ResourceSet productDetails = _metadata.AddResourceSet("ProductDetails", productDetail);
            ResourceSet categories = _metadata.AddResourceSet("Categories", category);
            ResourceSet suppliers = _metadata.AddResourceSet("Suppliers", supplier);
            ResourceSet persons = _metadata.AddResourceSet("Persons", person);
            ResourceSet personDetails = _metadata.AddResourceSet("PersonDetails", personDetail);
            ResourceSet advertisements = _metadata.AddResourceSet("Advertisements", advertisement);
            #endregion

            #region Derived Entity Types
            // We need to declare derived entity types after entity sets so that the resource set can
            // be set to the same as the base type - this is necessary for declaring nav props on derived 
            // types.

            // FeaturedProduct Entity Type
            ResourceType featuredProduct = _metadata.AddEntityType("FeaturedProduct", product);

            // Customer Entity Type
            ResourceType customer = _metadata.AddEntityType("Customer", person);
            _metadata.AddPrimitiveProperty(customer, "TotalExpense", typeof(decimal));

            // Employee Entity Type
            ResourceType employee = _metadata.AddEntityType("Employee", person);
            _metadata.AddPrimitiveProperty(employee, "EmployeeID", typeof(Int64));
            _metadata.AddPrimitiveProperty(employee, "HireDate", typeof(DateTime));
            _metadata.AddPrimitiveProperty(employee, "Salary", typeof(Single));
            #endregion

            #region Relationship
            // Person - PersonDetail association
            _metadata.AddNavigationProperty(person, "PersonDetail", false, personDetail, "Person", false);

            // Product - Categories association
            _metadata.AddNavigationProperty(product, "Categories", true, category, "Products", true);

            // Product - Suppliers association
            _metadata.AddNavigationProperty(product, "Supplier", false, supplier, "Products", true);

            // FeaturedProduct - Advertisement association
            _metadata.AddNavigationProperty(featuredProduct, "Advertisement", false, advertisement, "FeaturedProduct", false);

            // Product - ProductDetail association
            _metadata.AddNavigationProperty(product, "ProductDetail", false, productDetail, "Product", false);
            #endregion

            #region Operations
            // GetProductsByRating Service operation
            _metadata.AddServiceOperation(new ServiceOperation(
                "GetProductsByRating", ServiceOperationResultKind.QueryWithMultipleResults,
                product, products, "GET", new ServiceOperationParameter[] { new ServiceOperationParameter("rating", ResourceType.GetPrimitiveResourceType(typeof(Int16))) }));

            // Friendly Feed Mappings:
            product.AddEntityPropertyMappingAttribute(new EntityPropertyMappingAttribute("Name", SyndicationItemProperty.Title, SyndicationTextContentKind.Plaintext, false));
            product.AddEntityPropertyMappingAttribute(new EntityPropertyMappingAttribute("Description", SyndicationItemProperty.Summary, SyndicationTextContentKind.Plaintext, false));
            category.AddEntityPropertyMappingAttribute(new EntityPropertyMappingAttribute("Name", SyndicationItemProperty.Title, SyndicationTextContentKind.Plaintext, true));
            supplier.AddEntityPropertyMappingAttribute(new EntityPropertyMappingAttribute("Name", SyndicationItemProperty.Title, SyndicationTextContentKind.Plaintext, true));

            // Actions
            _actionProvider = new DSPActionProvider();

            // Add Discount action to Product
            _actionProvider.AddServiceAction(
                "Discount",
                DiscountProductAction,
                ResourceType.GetPrimitiveResourceType(typeof(double)),
                OperationParameterBindingKind.Always,
                new ServiceActionParameter("product", product),
                new ServiceActionParameter("discountPercentage", ResourceType.GetPrimitiveResourceType(typeof(int))));

            // Add non-bound action IncreaseSalaries
            _actionProvider.AddServiceAction(
                "IncreaseSalaries",
                IncreaseSalariesAction,
                null,
                OperationParameterBindingKind.Never,
                new ServiceActionParameter("percentage", ResourceType.GetPrimitiveResourceType(typeof(int))));

            #endregion
        }

        /// <summary>
        /// Create a DSPResource, and populate it with the given properties
        /// </summary>
        /// <param name="resourceType">The type of the resource</param>
        /// <param name="propertyNames">A list of property names</param>
        /// <param name="values">A list of values for the property names</param>
        /// <returns>A new instance of DSPResource</returns>
        private static DSPResource CreateResourceObject(ResourceType resourceType, string[] propertyNames, params object[] values)
        {
            Debug.Assert(propertyNames.Length == values.Length, "property and values length does not match");
            DSPResource res = new DSPResource(resourceType);

            for (int i = 0; i < propertyNames.Length; ++i)
            {
                res.SetValue(propertyNames[i], values[i]);
            }

            return res;
        }

        /// <summary>
        /// Create data for a new session
        /// </summary>
        /// <param name="metadata">The metadata to use</param>
        /// <returns>A new DSPContext containing the default data</returns>
        protected static DSPContext CreateDefaultData(DSPMetadata metadata)
        {
            DSPContext context = new DSPContext();

            ResourceSet productsSet, productDetailsSet, categoriesSet, suppliersSet, personsSet, personDetailsSet, advertisementSet;
            metadata.TryResolveResourceSet("Products", out productsSet);
            metadata.TryResolveResourceSet("ProductDetails", out productDetailsSet);
            metadata.TryResolveResourceSet("Categories", out categoriesSet);
            metadata.TryResolveResourceSet("Suppliers", out suppliersSet);
            metadata.TryResolveResourceSet("Persons", out personsSet);
            metadata.TryResolveResourceSet("PersonDetails", out personDetailsSet);
            metadata.TryResolveResourceSet("Advertisements", out advertisementSet);

            ResourceType addressType;
            metadata.TryResolveResourceType(metadata.ContainerNamespace + ".Address", out addressType);

            ResourceType customerType;
            metadata.TryResolveResourceType(metadata.ContainerNamespace + ".Customer", out customerType);

            ResourceType employeeType;
            metadata.TryResolveResourceType(metadata.ContainerNamespace + ".Employee", out employeeType);

            ResourceType featuredProductType;
            metadata.TryResolveResourceType(metadata.ContainerNamespace + ".FeaturedProduct", out featuredProductType);

            IList<DSPResource> products = context.GetResourceSetEntities(productsSet.Name);
            IList<DSPResource> productDetails = context.GetResourceSetEntities(productDetailsSet.Name);
            IList<DSPResource> categories = context.GetResourceSetEntities(categoriesSet.Name);
            IList<DSPResource> suppliers = context.GetResourceSetEntities(suppliersSet.Name);
            IList<DSPResource> persons = context.GetResourceSetEntities(personsSet.Name);
            IList<DSPResource> personDetails = context.GetResourceSetEntities(personDetailsSet.Name);
            IList<DSPResource> advertisements = context.GetResourceSetEntities(advertisementSet.Name);

            //
            // Categories
            //
            string[] categoryPropertyList = { "ID", "Name", "Products" };
            categories.Add(CreateResourceObject(categoriesSet.ResourceType, categoryPropertyList,
                0, "Food", new List<DSPResource>()));
            categories.Add(CreateResourceObject(categoriesSet.ResourceType, categoryPropertyList,
                1, "Beverages", new List<DSPResource>()));
            categories.Add(CreateResourceObject(categoriesSet.ResourceType, categoryPropertyList,
                2, "Electronics", new List<DSPResource>()));
            //
            // Suppliers
            //
            string[] addressPropertyList = { "Street", "City", "State", "ZipCode", "Country" };
            string[] supplierPropertyList = { "ID", "Name", "Concurrency", "Address", "Location", "Products" };
            suppliers.Add(CreateResourceObject(suppliersSet.ResourceType, supplierPropertyList,
                0, "Exotic Liquids", 0, CreateResourceObject(addressType, addressPropertyList, "NE 228th", "Sammamish", "WA", "98074", "USA"), GeographyPoint.Create(47.6316604614258, -122.03547668457), new List<DSPResource>()));
            suppliers.Add(CreateResourceObject(suppliersSet.ResourceType, supplierPropertyList,
                1, "Tokyo Traders", 0, CreateResourceObject(addressType, addressPropertyList, "NE 40th", "Redmond", "WA", "98052", "USA"), GeographyPoint.Create(47.6472206115723, -122.107711791992), new List<DSPResource>()));
            //
            // Products
            // 
            string[] productPropertyList = { "ID", "Name", "Description", "ReleaseDate", "DiscontinuedDate", "Rating", "Price", "ProductDetail", "Categories", "Supplier" };
            products.Add(CreateResourceObject(productsSet.ResourceType, productPropertyList,
                0, "Bread", "Whole grain bread", new DateTime(1992, 1, 1), null, (Int16)4, 2.5, null, new List<DSPResource>(), null));
            products.Add(CreateResourceObject(productsSet.ResourceType, productPropertyList,
                1, "Milk", "Low fat milk", new DateTime(1995, 10, 1), null, (Int16)3, 3.5, null, new List<DSPResource>(), null));
            products.Add(CreateResourceObject(productsSet.ResourceType, productPropertyList,
                2, "Vint soda", "Americana Variety - Mix of 6 flavors", new DateTime(2000, 10, 1), null, (Int16)3, 20.9, null, new List<DSPResource>(), null));
            products.Add(CreateResourceObject(productsSet.ResourceType, productPropertyList,
                3, "Havina Cola", "The Original Key Lime Cola", new DateTime(2005, 10, 1), new DateTime(2006, 10, 1), (Int16)3, 19.9, null, new List<DSPResource>(), null));
            products.Add(CreateResourceObject(productsSet.ResourceType, productPropertyList,
                4, "Fruit Punch", "Mango flavor, 8.3 Ounce Cans (Pack of 24)", new DateTime(2003, 1, 5), null, (Int16)3, 22.99, null, new List<DSPResource>(), null));
            products.Add(CreateResourceObject(productsSet.ResourceType, productPropertyList,
                5, "Cranberry Juice", "16-Ounce Plastic Bottles (Pack of 12)", new DateTime(2006, 8, 4), null, (Int16)3, 22.8, null, new List<DSPResource>(), null));
            products.Add(CreateResourceObject(productsSet.ResourceType, productPropertyList,
                6, "Pink Lemonade", "36 Ounce Cans (Pack of 3)", new DateTime(2006, 11, 5), null, (Int16)3, 18.8, null, new List<DSPResource>(), null));
            products.Add(CreateResourceObject(productsSet.ResourceType, productPropertyList,
                7, "DVD Player", "1080P Upconversion DVD Player", new DateTime(2006, 11, 15), null, (Int16)5, 35.88, null, new List<DSPResource>(), null));
            products.Add(CreateResourceObject(productsSet.ResourceType, productPropertyList,
                8, "LCD HDTV", "42 inch 1080p LCD with Built-in Blu-ray Disc Player", new DateTime(2008, 5, 8), null, (Int16)3, 1088.8, null, new List<DSPResource>(), null));

            // FeaturedProducts
            string[] featuredProductPropertyList = productPropertyList;
            products.Add(CreateResourceObject(featuredProductType, featuredProductPropertyList,
                9, "Lemonade", "Classic, refreshing lemonade (Single bottle)", new DateTime(1970, 1, 1), null, (Int16)7, 1.01, null, new List<DSPResource>(), null));
            products.Add(CreateResourceObject(featuredProductType, featuredProductPropertyList,
                10, "Coffee", "Bulk size can of instant coffee", new DateTime(1982, 12, 31), null, (Int16)1, 6.99, null, new List<DSPResource>(), null));

            //
            // Advertisements
            //
            string[] advertisementPropertyList = { "ID", "Name", "AirDate" };
            advertisements.Add(CreateResourceObject(advertisementSet.ResourceType, advertisementPropertyList,
                new Guid("F89DEE73-AF9F-4CD4-B330-DB93C25FF3C7"), "Old School Lemonade Store, Retro Style", new DateTime(2012, 11, 7)));
            advertisements.Add(CreateResourceObject(advertisementSet.ResourceType, advertisementPropertyList,
                new Guid("DB2D2186-1C29-4D1E-88EF-A127F521B9C6"), "Early morning start, need coffee", new DateTime(2000, 02, 29)));

            //
            // ProductDetails
            //
            string[] productDetailsPropertyList = { "ProductID", "Details", "Product" };
            productDetails.Add(CreateResourceObject(productDetailsSet.ResourceType, productDetailsPropertyList, 1, "Details of product 1", null));
            productDetails.Add(CreateResourceObject(productDetailsSet.ResourceType, productDetailsPropertyList, 3, "Details of product 3", null));
            productDetails.Add(CreateResourceObject(productDetailsSet.ResourceType, productDetailsPropertyList, 4, "Details of product 4", null));
            productDetails.Add(CreateResourceObject(productDetailsSet.ResourceType, productDetailsPropertyList, 8, "Details of product 8", null));

            // Product - ProductDetails Relationship
            foreach (var productDetail in productDetails)
            {
                int productID = (int)(productDetail.GetValue("ProductID"));
                productDetail.SetValue("Product", products[productID]);
                products[productID].SetValue("ProductDetail", productDetail);
            }

            //
            // Category-Product & Supplier-Product Relationships
            //
            var relationships = new[] {
                new { End1 = "C0", End2 = "P0" },
                new { End1 = "C0", End2 = "P1" },
                new { End1 = "C1", End2 = "P1" },
                new { End1 = "C1", End2 = "P2" },
                new { End1 = "C1", End2 = "P3" },
                new { End1 = "C1", End2 = "P4" },
                new { End1 = "C1", End2 = "P5" },
                new { End1 = "C1", End2 = "P6" },
                new { End1 = "C2", End2 = "P7" },
                new { End1 = "C2", End2 = "P8" },
                new { End1 = "S1", End2 = "P0" },
                new { End1 = "S0", End2 = "P1" },
                new { End1 = "S0", End2 = "P2" },
                new { End1 = "S0", End2 = "P3" },
                new { End1 = "S0", End2 = "P4" },
                new { End1 = "S0", End2 = "P5" },
                new { End1 = "S0", End2 = "P6" },
                new { End1 = "S1", End2 = "P7" },
                new { End1 = "S1", End2 = "P8" },
                new { End1 = "C1", End2 = "P9" },
                new { End1 = "C1", End2 = "P10" },
            };

            foreach (var r in relationships)
            {
                IList<DSPResource> set1 = r.End1.StartsWith("C") ? categories : suppliers;
                int id1 = Int32.Parse(r.End1.Substring(1));
                int id2 = Int32.Parse(r.End2.Substring(1));

                DSPResource left = set1.FirstOrDefault(e => (int)e.GetValue("ID") == id1);
                DSPResource product = products.FirstOrDefault(p => (int)p.GetValue("ID") == id2);

                IList<DSPResource> productList = left.GetValue("Products") as IList<DSPResource>;
                productList.Add(product);

                if (r.End1.StartsWith("C"))
                {
                    (product.GetValue("Categories") as IList<DSPResource>).Add(left);
                }
                else
                {
                    product.SetValue("Supplier", left);
                }

            }


            // PersonDetail
            string[] personDetailPropertyList = { "PersonID", "Age", "Gender", "Address", "Phone", "Person" };
            personDetails.Add(CreateResourceObject(personDetailsSet.ResourceType, personDetailPropertyList, 0, (Byte)21, false,
                CreateResourceObject(addressType, addressPropertyList, "2817 Milton Dr.", "Albuquerque", "NM", "87110", "USA"), "(505) 555-5939", null));
            personDetails.Add(CreateResourceObject(personDetailsSet.ResourceType, personDetailPropertyList, 1, (Byte)24, true,
                CreateResourceObject(addressType, addressPropertyList, "187 Suffolk Ln.", "Boise", "ID", "83720", "USA"), "(208) 555-8097", null));
            personDetails.Add(CreateResourceObject(personDetailsSet.ResourceType, personDetailPropertyList, 2, (Byte)23, true,
                CreateResourceObject(addressType, addressPropertyList, "P.O. Box 555", "Lander", "WY", "82520", "USA"), "(307) 555-4680", null));
            personDetails.Add(CreateResourceObject(personDetailsSet.ResourceType, personDetailPropertyList, 3, (Byte)30, true,
                CreateResourceObject(addressType, addressPropertyList, "89 Jefferson Way Suite 2", "Portland", "OR", "97201", "USA"), "(503) 555-3612", null));
            personDetails.Add(CreateResourceObject(personDetailsSet.ResourceType, personDetailPropertyList, 4, (Byte)40, true,
                CreateResourceObject(addressType, addressPropertyList, "55 Grizzly Peak Rd.", "Butte", "MT", "59801", "USA"), "(406) 555-5834", null));
            personDetails.Add(CreateResourceObject(personDetailsSet.ResourceType, personDetailPropertyList, 5, (Byte)30, true,
                CreateResourceObject(addressType, addressPropertyList, "87 Polk St. Suite 5", "San Francisco", "CA", "94117", "USA"), "(415) 555-5938", null));
            personDetails.Add(CreateResourceObject(personDetailsSet.ResourceType, personDetailPropertyList, 6, (Byte)28, false,
                CreateResourceObject(addressType, addressPropertyList, "89 Chiaroscuro Rd.", "Portland", "OR", "97219", "USA"), "(503) 555-9573", null));

            // Person
            string[] personPropertyList = { "ID", "Name", "PersonDetail" };
            persons.Add(CreateResourceObject(personsSet.ResourceType, personPropertyList, 0, "Paula Wilson", null));
            persons.Add(CreateResourceObject(personsSet.ResourceType, personPropertyList, 1, "Jose Pavarotti", null));
            persons.Add(CreateResourceObject(personsSet.ResourceType, personPropertyList, 2, "Art Braunschweiger", null));

            // Customers
            string[] customerPropertyList = personPropertyList.Concat(new[] { "TotalExpense" }).ToArray();
            persons.Add(CreateResourceObject(customerType, customerPropertyList, 3, "Liz Nixon", null, 99.99m));
            persons.Add(CreateResourceObject(customerType, customerPropertyList, 4, "Liu Wong", null, 199.99m));

            // Employees
            string[] employeePropertyList =
                personPropertyList.Concat(new[] { "EmployeeID", "HireDate", "Salary" }).ToArray();
            persons.Add(CreateResourceObject(employeeType, employeePropertyList, 5, "Jaime Yorres", null, 10001L, new DateTime(2000, 5, 30), 15000.0f));
            persons.Add(CreateResourceObject(employeeType, employeePropertyList, 6, "Fran Wilson", null, 10002L, new DateTime(2001, 1, 2), 12000.0f));

            // Person - PersonDetail Relationship
            for (int i = 0; i != persons.Count; ++i)
            {
                persons[i].SetValue("PersonDetail", personDetails[i]);
                personDetails[i].SetValue("Person", persons[i]);
            }

            // FeaturedProducts <-> Advertisements
            var featuredProducts = products.Where(p => p.ResourceType == featuredProductType);
            int counter = 0;
            foreach (var fp in featuredProducts)
            {
                var advertisement = advertisements.ElementAt(counter++);
                fp.SetValue("Advertisement", advertisement);
                advertisement.SetValue("FeaturedProduct", fp);
            }

            // Service Ops
            context.ServiceOperations.Add("GetProductsByRating", (args) => GetProductsByRating(context, (Int16)args.First()));

            // Initialise stream data
            int q = 0;
            foreach (var advertisement in advertisements)
            {
                _streamProvider.InitializeEntityMediaStream(advertisement, null, false, "text/plain", Encoding.UTF8.GetBytes("Test stream data " + q++));
            }

            foreach (var personDetail in personDetails)
            {
                _streamProvider.InitializeEntityNamedStream(personDetail, "Photo", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("Test named stream data " + q++));
            }

            _streamProvider.SaveStreams();

            return context;
        }

        #endregion

        /// <summary>
        /// Initialize the service
        /// </summary>
        /// <remarks>This method is called only once to initialize service-wide policies.</remarks>
        /// <param name="config">The configuration instance</param>
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            config.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
            config.DataServiceBehavior.IncludeAssociationLinksInResponse = true;
            config.UseVerboseErrors = true;

            SpatialImplementation.CurrentImplementation.Operations = _spatialOperations;

            config.AnnotationsBuilder = (model) =>
            {
                var myModel = new EdmModel();

                var container = model.FindDeclaredEntityContainer("DemoService");

                //Annotate Container with Display.Description vocabulary
                myModel.AddVocabularyAnnotation(new EdmValueAnnotation(container, new EdmValueTerm("Org.OData.Display.V1", "Description", EdmPrimitiveTypeKind.String), new EdmStringConstant("This is a sample OData service with vocabularies")));

                //Annotate Product Entity with Display.Description vocabulary
                var product = (IEdmEntityType)model.FindType("ODataDemo.Product");
                myModel.AddVocabularyAnnotation(new EdmValueAnnotation(product, new EdmValueTerm("Org.OData.Display.V1", "Description", EdmPrimitiveTypeKind.String), new EdmStringConstant("All Products available in the online store")));

                //Annotate Product.Name property with Display.DisplayName vocabulary
                var name = product.FindProperty("Name");
                myModel.AddVocabularyAnnotation(new EdmValueAnnotation(name, new EdmValueTerm("Org.OData.Display.V1", "DisplayName", EdmPrimitiveTypeKind.String), new EdmStringConstant("Product Name")));

                //Annotate Supplier EntitySet with Publish vocabulary
                var supplierSet = container.FindEntitySet("Suppliers");
                myModel.AddVocabularyAnnotation(new EdmValueAnnotation(supplierSet, new EdmValueTerm("Org.OData.Publication.V1", "PublisherName", EdmPrimitiveTypeKind.String), new EdmStringConstant("Microsoft Corp.")));
                myModel.AddVocabularyAnnotation(new EdmValueAnnotation(supplierSet, new EdmValueTerm("Org.OData.Publication.V1", "PublisherId", EdmPrimitiveTypeKind.String), new EdmStringConstant("MSFT")));
                myModel.AddVocabularyAnnotation(new EdmValueAnnotation(supplierSet, new EdmValueTerm("Org.OData.Publication.V1", "Keywords", EdmPrimitiveTypeKind.String), new EdmStringConstant("Inventory, Supplier, Advertisers, Sales, Finance")));
                myModel.AddVocabularyAnnotation(new EdmValueAnnotation(supplierSet, new EdmValueTerm("Org.OData.Publication.V1", "AttributionUrl", EdmPrimitiveTypeKind.String), new EdmStringConstant("http://www.odata.org/")));
                myModel.AddVocabularyAnnotation(new EdmValueAnnotation(supplierSet, new EdmValueTerm("Org.OData.Publication.V1", "AttributionDescription", EdmPrimitiveTypeKind.String), new EdmStringConstant("All rights reserved")));
                myModel.AddVocabularyAnnotation(new EdmValueAnnotation(supplierSet, new EdmValueTerm("Org.OData.Publication.V1", "DocumentationUrl ", EdmPrimitiveTypeKind.String), new EdmStringConstant("http://www.odata.org/")));
                myModel.AddVocabularyAnnotation(new EdmValueAnnotation(supplierSet, new EdmValueTerm("Org.OData.Publication.V1", "TermsOfUseUrl", EdmPrimitiveTypeKind.String), new EdmStringConstant("All rights reserved")));
                myModel.AddVocabularyAnnotation(new EdmValueAnnotation(supplierSet, new EdmValueTerm("Org.OData.Publication.V1", "PrivacyPolicyUrl", EdmPrimitiveTypeKind.String), new EdmStringConstant("http://www.odata.org/")));
                myModel.AddVocabularyAnnotation(new EdmValueAnnotation(supplierSet, new EdmValueTerm("Org.OData.Publication.V1", "LastModified", EdmPrimitiveTypeKind.String), new EdmStringConstant("4/2/2013")));
                myModel.AddVocabularyAnnotation(new EdmValueAnnotation(supplierSet, new EdmValueTerm("Org.OData.Publication.V1", "ImageUrl ", EdmPrimitiveTypeKind.String), new EdmStringConstant("http://www.odata.org/")));


                return new IEdmModel[] { myModel };
            };
        }

        /// <summary>
        /// Get Products by rating service operation
        /// </summary>
        /// <param name="rating">The rating parameter</param>
        /// <returns>A set of products with the supplied rating</returns>
        public static IQueryable<DSPResource> GetProductsByRating(DSPContext context, Int16 rating)
        {
            return from p in context.GetResourceSetEntities("Products").AsQueryable()
                   where ((Int16)p.GetValue("Rating")) == rating
                   select p;
        }

        /// <summary>
        /// Constructor of the service class
        /// </summary>
        /// <remarks>
        /// In here we try to inject the session ID into the URI for all entities
        /// The way we do it is by writing to IncomingMessageProperties["MicrosoftDataServicesRootUri"]
        /// and IncomingMessageProperties["MicrosoftDataServicesRequestUri"].
        /// The first one is the host name used to construct URIs, the second one overrides the request URI
        /// so links gets generated correctly.
        /// The URIs are only generated once per session, and is stored in the session variable.
        /// </remarks>
        public ODataService()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null &&
                !String.Equals(HttpContext.Current.Session.SessionID, ODataWebSessionIdManager.ODataReadOnlySession, StringComparison.InvariantCultureIgnoreCase))
            {
                if (WebOperationContext.Current.IncomingRequest.UriTemplateMatch != null)
                {
                    Uri serviceUri = HttpContext.Current.Session["ServiceUri"] as Uri;
                    Uri requestUri = null;

                    UriTemplateMatch match = WebOperationContext.Current.IncomingRequest.UriTemplateMatch;
                    string applicationPath = HttpContext.Current.Request.ApplicationPath;
                    if (serviceUri == null)
                    {
                        // create a service Uri that represent the current session
                        UriBuilder serviceUriBuilder = new UriBuilder(match.BaseUri);
                        Debug.Assert(serviceUriBuilder.Path.StartsWith("/V3"), "Expected the service URI to start with '/V3'.");
                        serviceUriBuilder.Path = applicationPath + "/(S(" + HttpContext.Current.Session.SessionID + "))" + serviceUriBuilder.Path.Substring(applicationPath.Length);

                        serviceUri = serviceUriBuilder.Uri;
                        HttpContext.Current.Session["ServiceUri"] = serviceUri;
                    }

                    if (serviceUri != null)
                    {
                        OperationContext.Current.IncomingMessageProperties["MicrosoftDataServicesRootUri"] = serviceUri;

                        UriBuilder requestUriBuilder = new UriBuilder(match.RequestUri);
                        Debug.Assert(requestUriBuilder.Path.StartsWith("/V3"), "Expected all request URIs to start with '/V3'.");
                        requestUriBuilder.Path = applicationPath + "/(S(" + HttpContext.Current.Session.SessionID + "))" + requestUriBuilder.Path.Substring(applicationPath.Length);
                        // because we have overwritten the Root URI, we need to make sure the request URI shares the same host
                        // (sometimes we have request URI resolving to a different host, if there are firewall re-directs
                        requestUriBuilder.Host = serviceUri.Host;

                        requestUri = requestUriBuilder.Uri;
                        OperationContext.Current.IncomingMessageProperties["MicrosoftDataServicesRequestUri"] = requestUri;
                    }
                }
            }
        }

        [ChangeInterceptor("Products")]
        [ChangeInterceptor("ProductDetails")]
        [ChangeInterceptor("Categories")]
        [ChangeInterceptor("Suppliers")]
        [ChangeInterceptor("Advertisements")]
        [ChangeInterceptor("Persons")]
        [ChangeInterceptor("PersonDetails")]
        public void OnUpdateOperation(object source, UpdateOperations action)
        {
            if (this.IsReadOnlyService())
            {
                throw new DataServiceException(403, "You are connected to a read-only data session. Update operations are not permitted for your session");
            }
        }

        /// <summary>
        /// GetService: make sure we use our update provider
        /// </summary>
        /// <param name="serviceType">The type of the service to get</param>
        /// <returns>The service provider instance</returns>
        public override object GetService(Type serviceType)
        {
            if (serviceType == typeof(IDataServiceUpdateProvider) && !this.IsReadOnlyService())
            {
                return new DSPRestrictedUpdateProvider(this.CurrentDataSource, this.Metadata);
            }
            else if (serviceType == typeof(IDataServiceActionProvider) && !this.IsReadOnlyService())
            {
                return _actionProvider;
            }
            else if (serviceType == typeof(IDataServiceStreamProvider) || serviceType == typeof(IDataServiceStreamProvider2))
            {
                return _streamProvider;
            }

            return base.GetService(serviceType);
        }

        /// <summary>
        /// CreateDataSource - generating new data context if one does not already exist for the current session
        /// </summary>
        /// <returns>a new instance of DSPContext</returns>
        protected override DSPContext CreateDataSource()
        {
            if (HttpContext.Current == null)
            {
                // no context?
                return CreateDefaultData(this.Metadata);
            }

            string sid = HttpContext.Current.Session.SessionID;

            DSPContext dataSource;
            if (!_dsCache.TryGetValue(sid, out dataSource))
            {
                if (_dsCache.Count > MaxCachedSessionItems)
                {
                    _dsCache.Clear();
                }

                dataSource = CreateDefaultData(this.Metadata);
                _dsCache.Add(sid, dataSource);
            }

            return dataSource;
        }

        /// <summary>
        /// Create default metadata
        /// </summary>
        /// <returns>An static instance of DSPMetadata</returns>
        protected override DSPMetadata CreateDSPMetadata()
        {
            if (_metadata == null)
            {
                CreateDefaultMetadata();
            }

            return _metadata;
        }

        private static object DiscountProductAction(DSPUpdateProvider updateProvider, IEnumerable<object> parameterTokens)
        {
            object product = parameterTokens.First();
            int discountPercentage = (int)parameterTokens.Skip(1).First();

            double price = (double)updateProvider.GetValue(product, "Price");
            price = price * (100 - discountPercentage) / 100;
            updateProvider.SetValue(product, "Price", price);

            return price;
        }

        private static object IncreaseSalariesAction(DSPUpdateProvider updateProvider, IEnumerable<object> parameterTokens)
        {
            DSPContext context;
            _dsCache.TryGetValue(HttpContext.Current.Session.SessionID, out context);
            var employees = context.GetResourceSetEntities("Persons").Where(p => p.ResourceType.Name == "Employee");
            foreach (var employee in employees)
            {
                updateProvider.SetValue(employee, "Salary", (Single)((Single)(employee.GetValue("Salary")) * (1 + (Int32)parameterTokens.First() / 100.0)));
            }
            return null;
        }

        /// <summary>
        /// Returns true if the service is read-only.
        /// </summary>
        /// <returns>true if the service is read-only.</returns>
        private bool IsReadOnlyService()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                String sessionId = HttpContext.Current.Session.SessionID;
                if (sessionId != ODataWebSessionIdManager.ODataReadOnlySession)
                {
                    return false;
                }
            }

            return true;
        }

        #region Update provider with constrain and concurrency support

        /// <summary>
        /// Update Provider that checks for/guards against data constraints
        /// </summary>
        internal class DSPRestrictedUpdateProvider : DSPUpdateProvider
        {
            public DSPRestrictedUpdateProvider(DSPContext dataContext, DSPMetadata metadata)
                : base(dataContext, metadata)
            {
            }

            /// <summary>
            /// Set the ETag property on Supplier to 0
            /// </summary>
            /// <param name="resource">The resource instance</param>
            protected override void ApplyResourceDefaultValue(DSPResource resource)
            {
                if (resource.ResourceType.Name == "Supplier")
                {
                    resource.SetValue("Concurrency", 0);
                }

                base.ApplyResourceDefaultValue(resource);
            }

            /// <summary>
            /// CreateResource: Limiting the number of entities per entity set to MaxEntityPerContainer
            /// </summary>
            /// <param name="containerName">The container name</param>
            /// <param name="fullTypeName">The type name</param>
            /// <returns>A new DSPResource instance</returns>
            public override object CreateResource(string containerName, string fullTypeName)
            {
                if (containerName != null)
                {
                    IList<DSPResource> resourceSetList = this.DataContext.GetResourceSetEntities(containerName);

                    if (resourceSetList != null && resourceSetList.Count > MaxEntityPerContainer)
                    {
                        throw new DataServiceException(400, "There are too many entities in the target set. Please start a new session and try again.");
                    }
                }

                return base.CreateResource(containerName, fullTypeName);
            }

            /// <summary>
            /// SetValue: Enforcing string length constrain, and ensure ETag are set/incremented properly
            /// </summary>
            /// <param name="targetResource"></param>
            /// <param name="propertyName"></param>
            /// <param name="propertyValue"></param>
            public override void SetValue(object targetResource, string propertyName, object propertyValue)
            {
                DSPResource dspTargetResource = ValidateDSPResource(targetResource);

                var rp = dspTargetResource.ResourceType.Properties.FirstOrDefault(p => p.Name == propertyName && p.ResourceType.FullName == "Edm.String");

                if (rp != null && propertyValue != null)
                {
                    if (propertyValue.ToString().Length > MaxStringPropertyLength)
                    {
                        throw new DataServiceException(400, "String property \"" + propertyName + "\" on this service has a maximum length restriction of 256 characters.");
                    }
                }

                //
                // Here we hard code the concurrency generation mechanism. Normally you would detect concurrency property, and update it's value
                // This is can also be done inside the property setter
                //
                if (dspTargetResource.ResourceType.Name == "Supplier")
                {

                    // generate concurrency value 
                    object currentValue = dspTargetResource.GetValue("Concurrency");

                    dspTargetResource.SetValue("Concurrency", currentValue == null ? 0 : (int)currentValue + 1);

                    if (propertyName == "Concurrency")
                    {
                        return;
                    }
                }

                base.SetValue(targetResource, propertyName, propertyValue);
            }
        }

        #endregion
    }
}
