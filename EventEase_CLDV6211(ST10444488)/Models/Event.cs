using System.ComponentModel.DataAnnotations;

namespace EventEase_CLDV6211_ST10444488_.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Required]
        public string EventName { get; set; }
        [Required]
        public DateTime EventDate { get; set; }
        public string Description { get; set; }


        //public int VenueID { get; set; }
        //public Venue Venue { get; set; }

        //public Venue? Venues { get; set; }
        public List<Booking>? Bookings { get; set; }
    }
}
