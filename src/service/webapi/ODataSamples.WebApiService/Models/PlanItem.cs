namespace ODataSamples.WebApiService.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PlanItem
    {
        [Key]
        public int PlanItemId { get; set; }

        public string ConfirmationCode { get; set; }

        public DateTimeOffset? StartsAt { get; set; }

        public DateTimeOffset? EndsAt { get; set; }

        public TimeSpan? Duration { get; set; }
    }

    public class PublicTransportation : PlanItem
    {
        public string SeatNumber { get; set; }
    }

    public class Event : PlanItem //TODO: [tiano] open type
    {
        [Required]
        public EventLocation OccursAt { get; set; }

        public string Description { get; set; }
    }

    public class Flight : PublicTransportation
    {
        [Required]
        public string FlightNumber { get; set; }

        [Required]
        public Airline Airline { get; set; }

        [Required]
        public Airport From { get; set; }

        [Required]
        public Airport To { get; set; }
    }
}
