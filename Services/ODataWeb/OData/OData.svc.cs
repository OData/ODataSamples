// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace ODataWeb.OData
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Services;
    using System.Data.Services.Common;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Web;
    using DataServiceProvider;

    [JSONPSupportBehavior]    
    [ServiceBehavior(IncludeExceptionDetailInFaults=true)]
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

        #region Metadata and Data Generation

        /// <summary>
        /// Generates the default metadata for the service
        /// </summary>
        private static void CreateDefaultMetadata()
        {
            _metadata = new DSPMetadata("DemoService", "ODataDemo");

            // Address Complex Type
            ResourceType address = _metadata.AddComplexType("Address");
            _metadata.AddPrimitiveProperty(address, "Street", typeof(string));
            _metadata.AddPrimitiveProperty(address, "City", typeof(string));
            _metadata.AddPrimitiveProperty(address, "State", typeof(string));
            _metadata.AddPrimitiveProperty(address, "ZipCode", typeof(string));
            _metadata.AddPrimitiveProperty(address, "Country", typeof(string));

            // Product Entity TYpe
            ResourceType product = _metadata.AddEntityType("Product");
            _metadata.AddKeyProperty(product, "ID", typeof(int));
            _metadata.AddPrimitiveProperty(product, "Name", typeof(string));
            _metadata.AddPrimitiveProperty(product, "Description", typeof(string));
            _metadata.AddPrimitiveProperty(product, "ReleaseDate", typeof(DateTime));
            _metadata.AddPrimitiveProperty(product, "DiscontinuedDate", typeof(DateTime?));
            _metadata.AddPrimitiveProperty(product, "Rating", typeof(int));
            _metadata.AddPrimitiveProperty(product, "Price", typeof(decimal));

            ResourceSet products = _metadata.AddResourceSet("Products", product);

            // Category Entity Type
            ResourceType category = _metadata.AddEntityType("Category");
            _metadata.AddKeyProperty(category, "ID", typeof(int));
            _metadata.AddPrimitiveProperty(category, "Name", typeof(string));

            ResourceSet categories = _metadata.AddResourceSet("Categories", category);

            // Supplier Entity Type
            ResourceType supplier = _metadata.AddEntityType("Supplier");
            _metadata.AddKeyProperty(supplier, "ID", typeof(int));
            _metadata.AddPrimitiveProperty(supplier, "Name", typeof(string));
            _metadata.AddComplexProperty(supplier, "Address", address);
            _metadata.AddETagProperty(supplier, "Concurrency", typeof(int));

            ResourceSet suppliers = _metadata.AddResourceSet("Suppliers", supplier);

            // Product - Categories association
            _metadata.AddNavigationProperty(product, "Category", false, category, "Products", true);

            // Product - Suppliers association
            _metadata.AddNavigationProperty(product, "Supplier", false, supplier, "Products", true);

            // GetProductsByRating Service operation
            _metadata.AddServiceOperation(new ServiceOperation(
                "GetProductsByRating", ServiceOperationResultKind.QueryWithMultipleResults,
                product, products, "GET", new ServiceOperationParameter[] { new ServiceOperationParameter("rating", ResourceType.GetPrimitiveResourceType(typeof(int))) }));

            // Friendly Feed Mappings:
            product.AddEntityPropertyMappingAttribute(new EntityPropertyMappingAttribute("Name", SyndicationItemProperty.Title, SyndicationTextContentKind.Plaintext, false));
            product.AddEntityPropertyMappingAttribute(new EntityPropertyMappingAttribute("Description", SyndicationItemProperty.Summary, SyndicationTextContentKind.Plaintext, false));
            category.AddEntityPropertyMappingAttribute(new EntityPropertyMappingAttribute("Name", SyndicationItemProperty.Title, SyndicationTextContentKind.Plaintext, true));
            supplier.AddEntityPropertyMappingAttribute(new EntityPropertyMappingAttribute("Name", SyndicationItemProperty.Title, SyndicationTextContentKind.Plaintext, true));
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

            ResourceSet productsSet, categoriesSet, suppliersSet;
            metadata.TryResolveResourceSet("Products", out productsSet);
            metadata.TryResolveResourceSet("Categories", out categoriesSet);
            metadata.TryResolveResourceSet("Suppliers", out suppliersSet);

            ResourceType addressType;
            metadata.TryResolveResourceType(metadata.ContainerNamespace + ".Address", out addressType);

            IList<DSPResource> products = context.GetResourceSetEntities(productsSet.Name);
            IList<DSPResource> categories = context.GetResourceSetEntities(categoriesSet.Name);
            IList<DSPResource> suppliers = context.GetResourceSetEntities(suppliersSet.Name);
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
            string[] supplierPropertyList = { "ID", "Name", "Concurrency", "Address", "Products" };
            suppliers.Add(CreateResourceObject(suppliersSet.ResourceType, supplierPropertyList,
                0, "Exotic Liquids", 0, CreateResourceObject(addressType, addressPropertyList, "NE 228th", "Sammamish", "WA", "98074", "USA"), new List<DSPResource>()));
            suppliers.Add(CreateResourceObject(suppliersSet.ResourceType, supplierPropertyList,
                1, "Tokyo Traders", 0, CreateResourceObject(addressType, addressPropertyList, "NE 40th", "Redmond", "WA", "98052", "USA"), new List<DSPResource>()));
            //
            // Products
            // 
            string[] productPropertyList = { "ID", "Name", "Description", "ReleaseDate", "DiscontinuedDate", "Rating", "Price", "Category", "Supplier" };
            products.Add(CreateResourceObject(productsSet.ResourceType, productPropertyList,
                0, "Bread", "Whole grain bread", new DateTime(1992, 1, 1), null, 4, 2.5m, null, null));
            products.Add(CreateResourceObject(productsSet.ResourceType, productPropertyList,
                1, "Milk", "Low fat milk", new DateTime(1995, 10, 1), null, 3, 3.5m, null, null));
            products.Add(CreateResourceObject(productsSet.ResourceType, productPropertyList,
                2, "Vint soda", "Americana Variety - Mix of 6 flavors", new DateTime(2000, 10, 1), null, 3, 20.9m, null, null));
            products.Add(CreateResourceObject(productsSet.ResourceType, productPropertyList,
                3, "Havina Cola", "The Original Key Lime Cola", new DateTime(2005, 10, 1), new DateTime(2006, 10, 1), 3, 19.9m, null, null));
            products.Add(CreateResourceObject(productsSet.ResourceType, productPropertyList,
                4, "Fruit Punch", "Mango flavor, 8.3 Ounce Cans (Pack of 24)", new DateTime(2003, 1, 5), null, 3, 22.99m, null, null));
            products.Add(CreateResourceObject(productsSet.ResourceType, productPropertyList,
                5, "Cranberry Juice", "16-Ounce Plastic Bottles (Pack of 12)", new DateTime(2006, 8, 4), null, 3, 22.8m, null, null));
            products.Add(CreateResourceObject(productsSet.ResourceType, productPropertyList,
                6, "Pink Lemonade", "36 Ounce Cans (Pack of 3)", new DateTime(2006, 11, 5), null, 3, 18.8m, null, null));
            products.Add(CreateResourceObject(productsSet.ResourceType, productPropertyList,
                7, "DVD Player", "1080P Upconversion DVD Player", new DateTime(2006, 11, 15), null, 3, 35.88m, null, null));
            products.Add(CreateResourceObject(productsSet.ResourceType, productPropertyList,
                8, "LCD HDTV", "42 inch 1080p LCD with Built-in Blu-ray Disc Player", new DateTime(2008, 5, 8), null, 3, 1088.8m, null, null));
            //
            // Relationships
            //
            var relationships = new[] {
                new { End1 = "C0", End2 = "P0" },
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

                product.SetValue(r.End1.StartsWith("C") ? "Category" : "Supplier", left);
            }

            // Service Ops

            context.ServiceOperations.Add("GetProductsByRating", (args) => GetProductsByRating(context, (int)args.First()));

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
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
            config.UseVerboseErrors = true;
        }

        /// <summary>
        /// Get Products by rating service operation
        /// </summary>
        /// <param name="rating">The rating parameter</param>
        /// <returns>A set of products with the supplied rating</returns>
        public static IQueryable<DSPResource> GetProductsByRating(DSPContext context, int rating)
        {
            return from p in context.GetResourceSetEntities("Products").AsQueryable()
                   where ((int)p.GetValue("Rating")) == rating
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

                    if (serviceUri == null)
                    {
                        // create a service Uri that represent the current session
                        UriBuilder serviceUriBuilder = new UriBuilder(match.BaseUri);
                        serviceUriBuilder.Path = "(S(" + HttpContext.Current.Session.SessionID + "))" + serviceUriBuilder.Path;

                        serviceUri = serviceUriBuilder.Uri;
                        HttpContext.Current.Session["ServiceUri"] = serviceUri;
                    }

                    if (serviceUri != null)
                    {
                        OperationContext.Current.IncomingMessageProperties["MicrosoftDataServicesRootUri"] = serviceUri;

                        UriBuilder requestUriBuilder = new UriBuilder(match.RequestUri);
                        requestUriBuilder.Path = "(S(" + HttpContext.Current.Session.SessionID + "))" + requestUriBuilder.Path;
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
        [ChangeInterceptor("Categories")]
        [ChangeInterceptor("Suppliers")]
        public void OnUpdateOperation(object source, UpdateOperations action)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                String sessionId = HttpContext.Current.Session.SessionID;
                if (sessionId != ODataWebSessionIdManager.ODataReadOnlySession)
                {
                    return;
                }
            }

            throw new DataServiceException(403, "You are connected to a read-only data session. Update operations are not permitted for your session");
        }

        /// <summary>
        /// GetService: make sure we use our update provider
        /// </summary>
        /// <param name="serviceType">The type of the service to get</param>
        /// <returns>The service provider instance</returns>
        public override object GetService(Type serviceType)
        {
            if (serviceType == typeof(IDataServiceUpdateProvider))
            {
                return new DSPRestrictedUpdateProvider(this.CurrentDataSource, this.Metadata);
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
        
        #region Update provider with constrain and concurrency support

        /// <summary>
        /// Update Provider that checks for/guards against data constrains
        /// </summary>
        internal class DSPRestrictedUpdateProvider : DSPUpdateProvider
        {
            public DSPRestrictedUpdateProvider(DSPContext dataContext, DSPMetadata metadata) : base(dataContext, metadata)
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
