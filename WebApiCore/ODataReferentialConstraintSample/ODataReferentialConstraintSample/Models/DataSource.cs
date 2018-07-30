// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace ODataReferentialConstraintSample.Models
{
    public class DataSource
    {
        private static IList<Customer> _customers;
        private static IList<Order> _orders;

        public static IList<Customer> Customers
        {
            get
            {
                Generate();
                return _customers;
            }
        }

        public static IList<Order> Orders
        {
            get
            {
                Generate();
                return _orders;
            }
        }

        private static void Generate()
        {
            if (_customers != null || _orders != null)
            {
                return;
            }

            _customers = new List<Customer>();
            _orders = new List<Order>();
            int orderId = 1;
            for (int i = 1; i <= 5; i++)
            {
                Customer customer = new Customer
                {
                    // Id = i,
                    Name = "Customer #" + i,
                    Orders = Enumerable.Range(1, 3).Select(e =>
                        new Order
                        {
                            OrderName = "Order #" + orderId++,
                            CustomerId = i
                        }).ToList()
                };

                foreach (var order in customer.Orders)
                {
                    order.Customer = customer;
                    _orders.Add(order);
                }

                _customers.Add(customer);
            }
        }
    }
}
