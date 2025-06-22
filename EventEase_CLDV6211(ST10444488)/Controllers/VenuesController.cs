using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase_CLDV6211_ST10444488_.Data;
using EventEase_CLDV6211_ST10444488_.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EventEase_CLDV6211_ST10444488_.Controllers
{
    public class VenuesController : Controller
    {
        private readonly BlobService _blobService;
        private readonly EventEase_CLDV6211_ST10444488_Context _context;

        public VenuesController(EventEase_CLDV6211_ST10444488_Context context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: Venues
        public async Task<IActionResult> Index()
        {
            return View(await _context.Venue.ToListAsync());
        }

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venue
                .FirstOrDefaultAsync(m => m.VenueID == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // GET: Venues/Create
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: Venues/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VenueID,VenueName,Capacity,Location")] Venue venue, IFormFile photoFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (photoFile != null && photoFile.Length > 0)
                    {
                        venue.ImageURL = await _blobService.UploadFileAsync(photoFile); 
                    }
                    
                    _context.Add(venue);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error uploading image: {ex.Message}";
                }
            }
            return View(venue);
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venue.FindAsync(id);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        // POST: Venues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueID,VenueName,Capacity,Location,ImageURL")] Venue venue, IFormFile photoFile)
        {
            if (id != venue.VenueID)
                return NotFound();

            var existingVenue = await _context.Venue.FindAsync(id);
            if (existingVenue == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (photoFile == null || photoFile.Length == 0)
                    {
                        Console.WriteLine("No new image uploaded.");
                    }
                    else
                    {
                        Console.WriteLine($"New image uploaded: {photoFile.FileName}");
                                                
                        if (!string.IsNullOrEmpty(existingVenue.ImageURL))
                        {
                            await _blobService.DeleteFileAsync(existingVenue.ImageURL);
                        }

                        existingVenue.ImageURL = await _blobService.UploadFileAsync(photoFile);
                    }

                    Console.WriteLine($"Updating venue: {venue.VenueName}, Capacity: {venue.Capacity}, Location: {venue.Location}");

                    existingVenue.VenueName = venue.VenueName;
                    existingVenue.Capacity = venue.Capacity;
                    existingVenue.Location = venue.Location;

                    _context.Update(existingVenue);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error updating venue: {ex.Message}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid input. Please check your entries.";
            }

            return View(venue);
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venue
                .FirstOrDefaultAsync(m => m.VenueID == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venue
                .Include(v => v.Bookings) 
                .FirstOrDefaultAsync(v => v.VenueID == id);
            if (venue == null)
                return NotFound();
            
            if (venue.Bookings != null && venue.Bookings.Any())
            {
                TempData["ErrorMessage"] = "Venue cannot be deleted because it has active bookings!";
                return RedirectToAction(nameof(Index)); 
            }
            _context.Venue.Remove(venue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Search(string searchTerm, DateTime? availabilityDate)
        {
            var venuesQuery = _context.Venue.AsQueryable();

            // Optional text filter (venue name or location)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                venuesQuery = venuesQuery.Where(v =>
                    v.VenueName.Contains(searchTerm) ||
                    v.Location.Contains(searchTerm));
            }

            if (availabilityDate.HasValue)
            {
                // Get booked venues on that date
                var bookedVenueIds = await _context.Booking
                    .Where(b => b.BookingDate.Date == availabilityDate.Value.Date)
                    .Select(b => b.VenueID)
                    .Distinct()
                    .ToListAsync();

                // Remove booked venues from results
                venuesQuery = venuesQuery.Where(v => !bookedVenueIds.Contains(v.VenueID));
            }

            var results = await venuesQuery.ToListAsync();
            ViewBag.AvailabilityDate = availabilityDate?.ToShortDateString();
            return View("Index", results); // Reuse your existing Index view
        }

        private bool VenueExists(int id)
        {
            return _context.Venue.Any(e => e.VenueID == id);
        }
    }
}
