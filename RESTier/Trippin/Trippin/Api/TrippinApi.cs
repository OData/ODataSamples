﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.OData.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Service.Sample.Trippin.Models;
using Microsoft.OData.Service.Sample.Trippin.Submit;
using Microsoft.Restier.AspNet.Model;
using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Model;
using Microsoft.Restier.Core.Submit;
using Microsoft.Restier.EntityFramework;

namespace Microsoft.OData.Service.Sample.Trippin.Api
{
    public class TrippinApi : EntityFrameworkApi<TrippinModel>
    {
        public TrippinModel ModelContext { get { return DbContext; } }

        [Resource]
        public Person Me
        {
            get
            {
                return DbContext.People
                    .Include("Friends")
                    .Include("Trips")
                    .Single(p => p.PersonId == 1);
            }
        }

        private IQueryable<Person> PeopleWithFriends
        {
            get { return ModelContext.People.Include("Friends"); }
        }

        /// <summary>
        /// Implements an action import.
        /// </summary>
        [UnboundOperation(Namespace = "Microsoft.OData.Service.Sample.Trippin.Models", OperationType = OperationType.Action)]
        public void ResetDataSource()
        {
            TrippinModel.ResetDataSource();
        }

        /// <summary>
        /// Action import - clean up all the expired trips.
        /// </summary>
        [UnboundOperation(Namespace = "Microsoft.OData.Service.Sample.Trippin.Models", OperationType = OperationType.Action)]
        public void CleanUpExpiredTrips()
        {
            // DO NOT ACTUALLY REMOVE THE TRIPS.
        }

        /// <summary>
        /// Bound action - set the end-up time of a trip.
        /// </summary>
        /// <param name="trip">The trip to update.</param>
        /// <returns>The trip updated.</returns>
        [BoundOperation(Namespace = "Microsoft.OData.Service.Sample.Trippin.Models", OperationType = OperationType.Action)]
        public Trip EndTrip(Trip trip)
        {
            // DO NOT ACTUALLY UPDATE THE TRIP.
            return trip;
        }

        /// <summary>
        /// Bound function - gets the number of friends of a person.
        /// </summary>
        /// <param name="person">The key of the binding person.</param>
        /// <returns>The number of friends of the person.</returns>
        [BoundOperation(Namespace = "Microsoft.OData.Service.Sample.Trippin.Models")]
        public int GetNumberOfFriends(Person person)
        {
            if (person == null)
            {
                return 0;
            }

            var personWithFriends = PeopleWithFriends.Single(p => p.PersonId == person.PersonId);
            return personWithFriends.Friends == null ? 0 : personWithFriends.Friends.Count;
        }

        /// <summary>
        /// Function import - gets the person with most friends.
        /// </summary>
        /// <returns>The person with most friends.</returns>
        [UnboundOperation(Namespace = "Microsoft.OData.Service.Sample.Trippin.Models", EntitySet = "People")]
        public Person GetPersonWithMostFriends()
        {
            Person result = null;

            foreach (var person in PeopleWithFriends)
            {
                if (person.Friends == null)
                {
                    continue;
                }

                if (result == null)
                {
                    result = person;
                }

                if (person.Friends.Count > result.Friends.Count)
                {
                    result = person;
                }
            }

            return result;
        }

        /// <summary>
        /// Function import - gets people with at least n friends.
        /// </summary>
        /// <param name="n">The minimum number of friends.</param>
        /// <returns>People with at least n friends.</returns>
        [UnboundOperation(Namespace = "Microsoft.OData.Service.Sample.Trippin.Models", EntitySet = "People")]
        public IEnumerable<Person> GetPeopleWithFriendsAtLeast(int n)
        {
            foreach (var person in PeopleWithFriends)
            {
                if (person.Friends == null)
                {
                    continue;
                }

                if (person.Friends.Count >= n)
                {
                    yield return person;
                }
            }
        }

        protected bool CanDeleteTrips()
        {
            return false;
        }

        internal class TrippinModelExtender : IModelBuilder
        {
            public IModelBuilder InnerModelBuilder { get; private set; }

            public TrippinModelExtender(IModelBuilder innerHandler)
            {
                this.InnerModelBuilder = innerHandler;
            }

            public IEdmModel GetModel(ModelContext context)
            {
                var model = InnerModelBuilder.GetModel(context);

                // Issue (todo): model returned by EFModelProducer.GetModelAsync is always null; how do we extend?
                if (model != null)
                {
                    // Set computed annotation
                    var tripType = (EdmEntityType)model.SchemaElements.Single(e => e.Name == "Trip");
                    var trackGuidProperty = tripType.DeclaredProperties.Single(prop => prop.Name == "TrackGuid");
                    var timeStampValueProp = model.EntityContainer.FindEntitySet("Airlines").EntityType().FindProperty("TimeStampValue");
                    var term = new EdmTerm("Org.OData.Core.V1", "Computed", EdmPrimitiveTypeKind.Boolean);
                    var anno1 = new EdmVocabularyAnnotation(trackGuidProperty, term, new EdmBooleanConstant(true));
                    var anno2 = new EdmVocabularyAnnotation(timeStampValueProp, term, new EdmBooleanConstant(true));
                    ((EdmModel)model).SetVocabularyAnnotation(anno1);
                    ((EdmModel)model).SetVocabularyAnnotation(anno2);
                }

                return model;
            }
        }

        public TrippinApi(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}