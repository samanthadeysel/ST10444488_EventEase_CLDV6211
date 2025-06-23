using System.ComponentModel.DataAnnotations;

namespace EventEase_CLDV6211_ST10444488_.Models
{
    public class Venue
    {
        [Key]
        public int VenueID { get; set; }
        [Required]
        public string VenueName { get; set; }

        [Required]
        [Range(1, 10000)]
        public int Capacity { get; set; }

        [Required]
        public string Location { get; set; }
        public string ImageURL { get; set; } //store blob url

        public List<Booking>? Bookings { get; set; }

        //public bool IsAvailable { get; set; } = true;
    }
}
