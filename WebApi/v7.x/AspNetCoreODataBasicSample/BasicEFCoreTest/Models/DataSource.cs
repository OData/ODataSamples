// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.Generic;

namespace BasicEFCoreTest.Models
{
    public static class DataSource
    {
        public static IList<Customer> GetCustomers()
        {
            Customer customerA = new Customer
            {
                FirstName = "ZZG",
                LastName = "Peter",
                UserName = "Sam",
                Age = 18,
                FavoriateColor = Color.Red,
                HomeAddress = new Address
                {
                    City = "Redmond",
                    Street = "156 AVE NE"
                },
                Order = new Order
                {
                    Price = 101m
                },
            };

            Customer customerB = new Customer
            {
                FirstName = "Jonier",
                LastName = "Alice",
                UserName = "Peter",
                Age = 19,
                FavoriateColor = Color.Red,
                HomeAddress = new Address
                {
                    City = "Bellevue",
                    Street = "Main St NE"
                },
                Order = new Order
                {
                    Price = 104m
                }
            };

            Customer customerC = new Customer
            {
                FirstName = "Jihan",
                LastName = "Ang",
                UserName = null,
                Age = 29,
                FavoriateColor = Color.Blue,
                HomeAddress = new Address
                {
                    City = "Hollewye",
                    Street = "Main St NE"
                },
                Order = new Order
                {
                    Price = 104m
                }
            };

            return new List<Customer>
            {
                customerA,
                customerB,
                customerC
            };
        }
    }
}
