namespace ODataSamples.WebApiService.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.OData.Builder;

    public class Trip
    {
        public Trip()
        {
            Tags = new List<string>();
            PlanItems = new List<PlanItem>();
        }

        [Key]
        public int TripId { get; set; }

        public Guid? ShareId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public float Budget { get; set; }

        public string Description { get; set; }

        public List<string> Tags { get; set; }

        [Required]
        public DateTimeOffset StartsAt { get; set; }

        [Required]
        public DateTimeOffset EndsAt { get; set; }

        [Contained]
        public List<PlanItem> PlanItems { get; set; }
    }
}
