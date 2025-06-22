using System.ComponentModel.DataAnnotations;

namespace EventEase_CLDV6211_ST10444488_.Models
{
    public class Venue
    {
        [Key]
        public int VenueID { get; set; }
        [Required]
        public string VenueName { get; set; }
        [Range(2, int.MaxValue)]
        public int Capacity { get; set; }
        public string Location { get; set; }
        public string ImageURL { get; set; }

        public List<Booking>? Bookings { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
