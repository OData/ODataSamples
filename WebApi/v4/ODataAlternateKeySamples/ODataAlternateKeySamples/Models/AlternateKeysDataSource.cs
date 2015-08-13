using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;

namespace ODataAlternateKeySamples.Models
{
    public static class AlternateKeysDataSource
    {
        public static EdmEntityObjectCollection Customers { get; private set; }

        public static EdmEntityObjectCollection Orders { get; private set; }

        public static EdmEntityObjectCollection People { get; private set; }

        static AlternateKeysDataSource()
        {
            IEdmModel model = AlternateKeyEdmModel.GetEdmModel();

            BuildCustomers(model);

            BuildOrderss(model);

            BuildPeople(model);
        }

        private static void BuildCustomers(IEdmModel model)
        {
            IEdmEntityType customerType = model.SchemaElements.OfType<IEdmEntityType>().First(e => e.Name == "Customer");

            IEdmEntityObject[] untypedCustomers = new IEdmEntityObject[6];
            for (int i = 1; i <= 5; i++)
            {
                dynamic untypedCustomer = new EdmEntityObject(customerType);
                untypedCustomer.ID = i;
                untypedCustomer.Name = new[] {"Tom", "Jerry", "Mike", "Ben", "Sam"}[i-1];
                untypedCustomer.SSN = "SSN-" + i + "-" + (100 + i);
                untypedCustomers[i - 1] = untypedCustomer;
            }

            // create a special customer for "PATCH"
            dynamic customer = new EdmEntityObject(customerType);
            customer.ID = 6;
            customer.Name = "John";
            customer.SSN = "SSN-6-T-006";
            untypedCustomers[5] = customer;

            IEdmCollectionTypeReference entityCollectionType =
                new EdmCollectionTypeReference(
                    new EdmCollectionType(
                        new EdmEntityTypeReference(customerType, isNullable: false)));

            Customers = new EdmEntityObjectCollection(entityCollectionType, untypedCustomers.ToList());
        }

        private static void BuildOrderss(IEdmModel model)
        {
            IEdmEntityType orderType = model.SchemaElements.OfType<IEdmEntityType>().First(e => e.Name == "Order");

            Guid[] tokes =
            {
                new Guid("196B3584-EF3D-41FD-90B4-76D59F9B929C"),
                new Guid("6CED5600-28BA-40EE-A2DF-E80AFADBE6C7"),
                new Guid("75036B94-C836-4946-8CC8-054CF54060EC"),
                new Guid("B3FF5460-6E77-4678-B959-DCC1C4937FA7"),
                new Guid("ED773C85-4E3C-4FC4-A3E9-9F1DA0A626DA"),
                new Guid("E9CC3D9F-BC80-4D43-8C3E-ED38E8C9A8B6")
            };

            IEdmEntityObject[] untypedOrders = new IEdmEntityObject[6];
            for (int i = 0; i < 6; i++)
            {
                dynamic untypedOrder = new EdmEntityObject(orderType);
                untypedOrder.OrderId = i;
                untypedOrder.Name = string.Format("Order-{0}", i);
                untypedOrder.Token = tokes[i];
                untypedOrder.Amount = 10 * (i + 1) - i;
                untypedOrders[i] = untypedOrder;
            }

            IEdmCollectionTypeReference entityCollectionType =
                new EdmCollectionTypeReference(
                    new EdmCollectionType(
                        new EdmEntityTypeReference(orderType, isNullable: false)));

            Orders = new EdmEntityObjectCollection(entityCollectionType, untypedOrders.ToList());
        }

        private static void BuildPeople(IEdmModel model)
        {
            IEdmEntityType personType = model.SchemaElements.OfType<IEdmEntityType>().First(e => e.Name == "Person");

            IEdmEntityObject[] untypedPeople = new IEdmEntityObject[6];
            for (int i = 0; i < 6; i++)
            {
                dynamic untypedPerson = new EdmEntityObject(personType);
                untypedPerson.ID = i;
                untypedPerson.Country = new[] {"England", "China", "United States", "Russia", "Japan", "Korea"}[i];
                untypedPerson.Passport = new[] {"1001", "2010", "9999", "3199992", "00001", "8110"}[i];
                untypedPeople[i] = untypedPerson;
            }

            IEdmCollectionTypeReference entityCollectionType =
                new EdmCollectionTypeReference(
                    new EdmCollectionType(
                        new EdmEntityTypeReference(personType, isNullable: false)));

            People = new EdmEntityObjectCollection(entityCollectionType, untypedPeople.ToList());
        }

    }
}