// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Spatial;

namespace Microsoft.OData.Service.Sample.Spatial.Models
{
    public class SpatialModel : DbContext
    {
        static SpatialModel()
        {
            Database.SetInitializer(new SpatialDatabaseInitializer());
        }

        public SpatialModel()
            : base("name=SpatialModel")
        {
        }

        public DbSet<Person> People { get; set; }

        private static SpatialModel instance;
        public static SpatialModel Instance
        {
            get
            {
                if (instance == null)
                {
                    ResetDataSource();
                }
                return instance;
            }
        }

        public static void ResetDataSource()
        {
            instance = new SpatialModel();

            // Discard all local changes, and reload data from DB, them remove all
            foreach (var x in instance.People)
            {
                // Discard local changes for the person..
                instance.Entry(x).State = EntityState.Detached;
            }

            instance.People.RemoveRange(instance.People);

            // This is to set the People Id from 0
            instance.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('People', RESEED, 0)");

            instance.SaveChanges();

            #region People

            #region Friends russellwhyte & scottketchum & ronaldmundy
            var person1 = new Person
            {
                PersonId = 1,
                FirstName = "Russell",
                LastName = "Whyte",
                UserName = "russellwhyte",
                DbLocation = DbGeography.FromText("POINT(52.808019 -1.345367)", 4326),
                DbLineString = DbGeography.LineFromText("LINESTRING (30 10, 10 30, 40 40)", 4326)
            };

            var person2 = new Person
            {
                PersonId = 2,
                FirstName = "Scott",
                LastName = "Ketchum",
                UserName = "scottketchum",
                DbLocation = DbGeography.FromText("POINT(52.808019 -1.345367)", 4326),
                DbLineString = DbGeography.LineFromText("LINESTRING (40 10, 10 30, 40 40)", 4326)
            };

            var person3 = new Person
            {
                PersonId = 3,
                FirstName = "Ronald",
                LastName = "Mundy",
                UserName = "ronaldmundy",
                DbLocation = DbGeography.FromText("POINT(52.808019 -1.345367)", 4326)
            };

            var person4 = new Person
            {
                PersonId = 4,
                FirstName = "Javier",
                UserName = "javieralfred",
                DbLocation = DbGeography.FromText("POINT(52.808019 -1.345367)", 4326)
            };

            #endregion

            instance.People.AddRange(new List<Person>
            {
                person1,
                person2,
                person3,
                person4,
            });

            #endregion

            instance.SaveChanges();
        }
    }

    class SpatialDatabaseInitializer : DropCreateDatabaseAlways<SpatialModel>
    {
        protected override void Seed(SpatialModel context)
        {
            SpatialModel.ResetDataSource();
        }
    }
}
