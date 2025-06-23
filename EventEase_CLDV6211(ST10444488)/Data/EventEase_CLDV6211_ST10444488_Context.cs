using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EventEase_CLDV6211_ST10444488_.Models;

namespace EventEase_CLDV6211_ST10444488_.Data
{
    public class EventEase_CLDV6211_ST10444488_Context : DbContext
    {
        public DbSet<Event> Event { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<Venue> Venue { get; set; }
        public DbSet<EventType> EventType { get; set; }

        public EventEase_CLDV6211_ST10444488_Context(DbContextOptions options)
            : base(options) { }
    }
}
