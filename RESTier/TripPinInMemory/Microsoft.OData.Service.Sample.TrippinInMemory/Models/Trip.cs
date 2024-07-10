// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Trippin
{
    public class Trip
    {
        public int TripId { get; set; }

        public Guid ShareId { get; set; }

        public string Name { get; set; }

        public float Budget { get; set; }

        public string Description { get; set; }

        public ICollection<string> Tags { get; set; }

        public DateTimeOffset StartsAt { get; set; }

        public DateTimeOffset EndsAt { get; set; }

        public virtual ICollection<PlanItem> PlanItems { get; set; }

        public Trip()
        {
            Tags = new List<string>();
            PlanItems = new List<PlanItem>();
        }

        public object Clone()
        {
            Trip newTrip = new Trip()
            {
                TripId = 0,//Should reset the trip id value.
                ShareId = this.ShareId,
                Name = this.Name,
                Budget = this.Budget,
                Description = this.Description,
                StartsAt = this.StartsAt,
                EndsAt = this.EndsAt
            };

            if (this.Tags != null)
            {
                foreach (var tag in this.Tags)
                {
                    newTrip.Tags.Add(tag);
                }
            }

            if (this.PlanItems != null)
            {
                foreach (var planItem in this.PlanItems)
                {
                    newTrip.PlanItems.Add(planItem.Clone() as PlanItem);
                }
            }

            return newTrip;
        }
    }
};
