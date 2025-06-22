using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase_CLDV6211_ST10444488_.Data;
using EventEase_CLDV6211_ST10444488_.Models;

namespace EventEase_CLDV6211_ST10444488_.Controllers
{
    public class BookingsController : Controller
    {
        private readonly EventEase_CLDV6211_ST10444488_Context _context;

        public BookingsController(EventEase_CLDV6211_ST10444488_Context context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var eventEase_CLDV6211_ST10444488_Context = _context.Booking.Include(b => b.Events).Include(b => b.Venues);
            return View(await eventEase_CLDV6211_ST10444488_Context.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Events)
                .Include(b => b.Venues)
                .FirstOrDefaultAsync(m => m.BookingID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventName");
            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingID,EventID,VenueID,BookingDate")] Booking booking)
        {
            bool bookingExists = await _context.Booking
                .AnyAsync(b => b.VenueID == booking.VenueID && b.BookingDate.Date == booking.BookingDate.Date);

            if (bookingExists)
            {
                ViewData["ErrorMessage"] = "This venue is already booked on this date. Please choose another date or venue.";
                return View(booking);
            }

            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventName", booking.EventID);
            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueID);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventName", booking.EventID);
            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingID,EventID,VenueID,BookingDate")] Booking booking)
        {
            if (id != booking.BookingID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventName", booking.EventID);
            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueID);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Events)
                .Include(b => b.Venues)
                .FirstOrDefaultAsync(m => m.BookingID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking != null)
            {
                if (booking.Venues != null || booking.Events != null)
                {
                    TempData["ErrorMessage"] = "This booking cannot be deleted as it is linked to a venue or event.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Booking.Remove(booking);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        //public async Task<IActionResult> Search(string searchTerm, DateTime? startDate, DateTime? endDate, string location, int? eventTypeId, bool onlyAvailable=false)
        //{

        //    ViewBag.Venues = await _context.Venue
        //        .Select(v => v.Location)
        //        .Distinct()
        //        .ToListAsync();

        //    var bookingsQuery = _context.Booking
        //        .Include(b => b.Venues)
        //        .Include(b => b.Events)
        //        .Include(b => b.EventType)
        //        .AsQueryable();

        //    if (!string.IsNullOrEmpty(searchTerm))
        //    {
        //        bookingsQuery = bookingsQuery.Where(b =>
        //            b.BookingID.ToString().Contains(searchTerm) ||
        //            (b.Events != null && b.Events.EventName.Contains(searchTerm))
        //        );
        //    }

        //    if (startDate.HasValue)
        //    {
        //        bookingsQuery = bookingsQuery.Where(b => b.BookingDate >= startDate.Value);
        //    }

        //    if (endDate.HasValue)
        //    {
        //        bookingsQuery = bookingsQuery.Where(b => b.BookingDate <= endDate.Value);
        //    }

        //    if (eventTypeId.HasValue)
        //    {
        //        bookingsQuery = bookingsQuery.Where(b => b.EventTypeID == eventTypeId.Value);
        //    }

        //    if (onlyAvailable)
        //    {
        //        bookingsQuery = bookingsQuery.Where(b => b.Venues.IsAvailable);
        //    }

        //    if (!string.IsNullOrEmpty(location))
        //    {
        //        bookingsQuery = bookingsQuery.Where(b => b.Venues != null && b.Venues.Location == location);
        //    }

        //    var bookings = await bookingsQuery.ToListAsync();
        //    return View(bookings);
        //}

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.BookingID == id);
        }
    }
}
