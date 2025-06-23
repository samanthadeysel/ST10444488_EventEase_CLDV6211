using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase_CLDV6211_ST10444488_.Data;
using EventEase_CLDV6211_ST10444488_.Models;
using Azure.Storage.Blobs;
using System.Net;
using Azure.Storage.Blobs.Models;

namespace EventEase_CLDV6211_ST10444488_.Controllers
{
    public class VenuesController : Controller
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly EventEase_CLDV6211_ST10444488_Context _context;
        private readonly string _containerName;

        public VenuesController(EventEase_CLDV6211_ST10444488_Context context, IConfiguration config)
        {
            _context = context;
            _blobServiceClient = new BlobServiceClient(config["AzureBlobStorage:ConnectionString"]);
            _containerName = config["AzureBlobStorage:ContainerName"];
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
        public async Task<IActionResult> Create(Venue venue, IFormFile photoFile)
        {
            if (photoFile == null || photoFile.Length == 0)
            {
                ModelState.AddModelError("photoFile", "Please upload a venue image.");
                return View(venue);
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(photoFile.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("photoFile", "Only JPG and PNG image files are allowed.");
                return View(venue);
            }

            if (!ModelState.IsValid)
            {
                foreach (var modelError in ModelState)
                {
                    Console.WriteLine($"🔻 Field: {modelError.Key}");
                    foreach (var error in modelError.Value.Errors)
                    {
                        Console.WriteLine($"❌ {error.ErrorMessage}");
                    }
                }

                return View(venue);
            }

            var container = _blobServiceClient.GetBlobContainerClient(_containerName);
            await container.CreateIfNotExistsAsync();

            var fileName = Guid.NewGuid() + Path.GetExtension(photoFile.FileName);
            var blob = container.GetBlobClient(fileName);

            using var stream = photoFile.OpenReadStream();
            await blob.UploadAsync(stream, overwrite: true);

            venue.ImageURL = blob.Uri.ToString();

            Console.WriteLine("👉 Venue ready to save:");
            Console.WriteLine($"Name: {venue.VenueName}, Capacity: {venue.Capacity}, Location: {venue.Location}, Image: {venue.ImageURL}");

            _context.Add(venue);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Venue venue)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(venue);
        //    }

        //    _context.Add(venue);
        //    await _context.SaveChangesAsync();

        //    Console.WriteLine("🔥 SAVED Venue:");
        //    Console.WriteLine($"Name: {venue.VenueName}, Capacity: {venue.Capacity}, Location: {venue.Location}");

        //    return RedirectToAction(nameof(Index));
        //}

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venue
            .Include(v => v.Bookings)
            .FirstOrDefaultAsync(v => v.VenueID == id);

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
        public async Task<IActionResult> Edit(int id, Venue venue, IFormFile photoFile)
        {
            if (id != venue.VenueID)
                return NotFound();

            var existingVenue = await _context.Venue.FindAsync(id);
            if (existingVenue == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(venue);

            // Handle image replacement if a new file was uploaded
            if (photoFile != null && photoFile.Length > 0)
            {
                var container = _blobServiceClient.GetBlobContainerClient(_containerName);
                await container.CreateIfNotExistsAsync();

                // Delete the old image if it exists
                if (!string.IsNullOrEmpty(existingVenue.ImageURL))
                {
                    var oldBlobName = Path.GetFileName(new Uri(existingVenue.ImageURL).LocalPath);
                    var oldBlob = container.GetBlobClient(oldBlobName);
                    await oldBlob.DeleteIfExistsAsync();
                }

                // Upload the new image
                var fileName = Guid.NewGuid() + Path.GetExtension(photoFile.FileName);
                var newBlob = container.GetBlobClient(fileName);

                using var stream = photoFile.OpenReadStream();
                await newBlob.UploadAsync(stream, overwrite: true);

                existingVenue.ImageURL = newBlob.Uri.ToString();
            }

            // Update fields
            existingVenue.VenueName = venue.VenueName;
            existingVenue.Capacity = venue.Capacity;
            existingVenue.Location = venue.Location;

            _context.Update(existingVenue);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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
            var venue = await _context.Venue.FindAsync(id);
            if (venue == null)
                return NotFound();

            var container = _blobServiceClient.GetBlobContainerClient(_containerName);

            var blobUri = new Uri(venue.ImageURL);
            var blobName = WebUtility.UrlDecode(blobUri.Segments.Last());

            var blob = container.GetBlobClient(blobName);
            if(_context.Venue.Count(v=>v.ImageURL == venue.ImageURL) <= 1)
            {
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            }

            _context.Venue.Remove(venue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

            //if (venue.Bookings != null && venue.Bookings.Any())
            //{
            //    TempData["ErrorMessage"] = "Venue cannot be deleted — active bookings exist.";
            //    return RedirectToAction(nameof(Index));
            //}

            //try
            //{
            //    if (!string.IsNullOrEmpty(venue.ImageURL))
            //    {
            //        await _blobServiceClient.DeleteFileAsync(venue.ImageURL);
            //    }

            //    _context.Venue.Remove(venue);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //catch (Exception ex)
            //{
            //    TempData["ErrorMessage"] = $"Error deleting venue: {ex.Message}";
            //    return RedirectToAction(nameof(Index));
            //}
        }

        public async Task<IActionResult> Search(string searchTerm, DateTime? availabilityDate)
        {
            var venuesQuery = _context.Venue.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                venuesQuery = venuesQuery.Where(v =>
                    v.VenueName.Contains(searchTerm) ||
                    v.Location.Contains(searchTerm));
            }

            if (availabilityDate.HasValue)
            {
                var bookedVenueIds = await _context.Booking
                    .Where(b => b.BookingDate.Date == availabilityDate.Value.Date)
                    .Select(b => b.VenueID)
                    .ToListAsync();

                venuesQuery = venuesQuery.Where(v => !bookedVenueIds.Contains(v.VenueID));
            }

            var results = await venuesQuery.ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.AvailabilityDate = availabilityDate?.ToString("yyyy-MM-dd");

            return View("Index", results);
        }

        private bool VenueExists(int id)
        {
            return _context.Venue.Any(e => e.VenueID == id);
        }
    }
}
