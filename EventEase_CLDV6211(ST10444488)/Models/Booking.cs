using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace EventEase_CLDV6211_ST10444488_.Models
{
    public class Booking
    {
        public int BookingID { get; set; }

        public int EventID { get; set; }

        public int VenueID { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }



        //public int? EventID { get; set; }
        //public int? VenueID { get; set; }

        public Venue? Venues { get; set; }
        public Event? Events { get; set; }
    }
}
