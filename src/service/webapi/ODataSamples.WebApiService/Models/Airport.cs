namespace ODataSamples.WebApiService.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Airport
    {
        [Key]
        public string IcaoCode { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public AirportLocation Location { get; set; }

        [Required]
        public string IataCode { get; set; }
    }
}