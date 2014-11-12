namespace ODataSamples.WebApiService.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.OData.Builder;

    public class Person //TODO: [tiano] open type
    {
        public Person()
        {
            Emails = new List<string>();
            AddressInfo = new List<Location>();

            Trips = new List<Trip>();
            Friends = new List<Person>();
            Concurrency = DateTime.UtcNow.Ticks;
        }

        [Key]
        public string UserName { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public PersonGender Gender { get; set; }

        //public Stream Photo { get; set; }

        public string Introduction { get; set; }

        public List<string> Emails { get; set; }

        public List<Location> AddressInfo { get; set; }

        [Contained]
        public List<Trip> Trips { get; set; }

        public List<Person> Friends { get; set; }

        [ConcurrencyCheck]
        public long Concurrency { get; set; }
    }
}