using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using WildStays.Models;

namespace WildStays.DAL;

public class ItemRepository : IItemRepository
{
    private readonly DatabaseDbContext _db;
    private readonly IWebHostEnvironment _webHostEnvironment;

    private readonly ILogger<ItemRepository> _logger;

    public ItemRepository(DatabaseDbContext db, ILogger<ItemRepository> logger, IWebHostEnvironment webHostEnvironment)
    {
        _db = db;
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
    }

    //Same method as in the demo from module 6, gets all listings.
    public async Task<IEnumerable<Listing>?> GetAll()
    {
        try
        {
            return await _db.Listings.ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[ItemRepository] failed to get all items, error message: {e}", e.Message);
            return null;
        }

    }
    //Same method as in the demo from module 6, gets a listing based on the item id.
    public async Task<Listing?> GetItemById(int id)
    {
        try
        {
            return await _db.Listings.FindAsync(id);
        }
        catch (Exception e)
        {
            _logger.LogError("[ItemRepository] failed to get item by Id {ItemId:0000}, error message: {e}", id, e.Message);
            return null;
        }

    }

    //Same method as in the demo from module 6, createas a listing to the database.
    public async Task<bool> Create(Listing listing, List<IFormFile> Images)
    {
        try
        {
            var images = new List<Image>();

            foreach (var imageFile in Images)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    var image = new Image
                    {
                        FilePath = "/uploads/" + uniqueFileName,
                        ListingId = listing.Id
                    };

                    images.Add(image);

                    _logger.LogInformation("Image uploaded: {FilePath}, ListingId: {ListingId}", image.FilePath, image.ListingId);
                }
                else
                {
                    _logger.LogWarning("Skipped an empty image file.");
                }
            }

            // Log the image paths in the listing
            _logger.LogInformation("Image paths in the listing: {@ImagePaths}", images.Select(img => img.FilePath).ToList());

            listing.Images = images;

            _db.Listings.Add(listing);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Listing created successfully.");

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[ItemRepository] failed to create the listing {@listing}, error message: {e}", listing, e.Message);
            return false;
        }
    }



    //Modified the method from the module as i was getting detach issues when updating, similar to the delete method.
    public async Task<bool> Update(Listing listing)
    {
        try
        {
            var existingListing = await _db.Listings.FirstOrDefaultAsync(l => l.Id == listing.Id);
            if (existingListing == null)
            {
                return false;
            }

            // Update the properties of the existing listing with the new values
            existingListing.Name = listing.Name;
            existingListing.Place = listing.Place;
            existingListing.Description = listing.Description;
            existingListing.Type = listing.Type;
            existingListing.Price = listing.Price;
            existingListing.Guests = listing.Guests;
            existingListing.Bedrooms = listing.Bedrooms;
            existingListing.Bathrooms = listing.Bathrooms;
            existingListing.StartDate = listing.StartDate;
            existingListing.EndDate = listing.EndDate;

            // Save the changes to the database
            await _db.SaveChangesAsync();

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[ItemRepository] failed to update the listing {@listing}, error message: {e}", listing, e.Message);
            return false;
        }
    }

    public async Task<bool> ManageImages(Listing listing, List<IFormFile> newImages, int? imageToDeleteId)
    {
        try
        {
            var existingListing = await _db.Listings.Include(l => l.Images).FirstOrDefaultAsync(l => l.Id == listing.Id);
            if (existingListing == null)
            {
                return false;
            }

            if (imageToDeleteId.HasValue)
            {
                // Delete image
                var imageToDelete = existingListing.Images.FirstOrDefault(img => img.Id == imageToDeleteId);
                if (imageToDelete != null)
                {
                    existingListing.Images.Remove(imageToDelete);
                }
            }

            if (newImages != null && newImages.Count > 0)
            {
                // Handle image upload
                var imagesToAdd = new List<Image>();

                foreach (var imageFile in newImages)
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        var image = new Image
                        {
                            FilePath = "/uploads/" + uniqueFileName,
                            ListingId = existingListing.Id
                        };

                        imagesToAdd.Add(image);
                    }
                }

                existingListing.Images.AddRange(imagesToAdd);
            }

            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[ItemRepository] failed to manage images for listing ID {ListingId:0000}, error message: {e}", listing.Id, e.Message);
            return false;
        }
    }



    public async Task<bool> Delete(int id)
    {
        try
        {
            var listing = await _db.Listings.FindAsync(id);

            if (listing == null)
            {
                _logger.LogError("[ItemRepository] could not find the item with item Id {ItemId:0000}", id);
                return false;
            }

            // Gets images based on the listing id
            var images = await GetImagesByListingId(id);
            //Gets the fullpath
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath);
            // Goes through all files belonging to the listing
            foreach (var image in images)
            {
                //Deletes files
                File.Delete(uploadsFolder + image.FilePath);
                
            }

            // Remove the images from the database
            _db.Images.RemoveRange(images);

            // Remove the listing
            _db.Listings.Remove(listing);

            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[ItemRepository] Failed to delete the item with the Id {ItemId:0000}, error message: {e}", id, e.Message);
            return false;
        }
    }


    //Method to get a listing by userId, used to show only show a user their listings
    public async Task<IEnumerable<Listing>?> GetListingsByUserId(string userId)
    {
        try
        {
            //Lamda expression to fetch listings with userId matching the currently logged in users Id.
            return await _db.Listings.Where(l => l.UserId == userId).ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[ItemRepository] Failed to get the listings based on the spescific user(Id), error message: {e}", e.Message);
            return null;
        }
    }


    //Method to get reservations by user id.
    public async Task<IEnumerable<Reservation>?> GetReservationByUserId(string userId)
    {
        try
        {
            //Also lamda expressions to get reservations with the users userId, but also gets information from the listings database assosiated with the reservation.
            return await _db.Reservations.Where(l => l.UserId == userId).Include(r => r.Listing).ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[ItemRepository] Failed to find the reservation based on UserID error message: {e}", e.Message);
            return null;
        }

    }
    public async Task<IEnumerable<Image>> GetImagesByListingId(int listingId)
    {
        try
        {
            // Use EF Core to query the Images with the specified ListingId
            return await _db.Images.Where(i => i.ListingId == listingId).ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[ItemRepository] Failed to get images by ListingId {ListingId:0000}, error message: {e}", listingId, e.Message);
            return null;
        }
    }


    //Creates a reservation
    public async Task<bool> CreateReservation(Reservation reservation)
    {
        try
        {
            // Check if the listing is available between the specified dayes. Checks both the reservation and listing databases if the dates are in the reservations database, or are not in the listing.
            bool isAvailable = !_db.Reservations.Any(r =>
                r.ListingId == reservation.ListingId &&
                ((reservation.StartDate >= r.StartDate && reservation.StartDate <= r.EndDate) ||
                 (reservation.EndDate >= r.StartDate && reservation.EndDate <= r.EndDate)));
            //If available adds the reservation to the database
            if (isAvailable)
            {
                _db.Reservations.Add(reservation);
                await _db.SaveChangesAsync();
                return true;
            }
            else
            {
                return false; // Reservation not available
            }
        }
        catch (Exception e)
        {
            _logger.LogError("[ItemRepository] reservation creation failed for reservation {@reservation}, error message: {e}", reservation, e.Message);
            return false;
        }
    }
    //Checks if the startdate is after todays date
    public bool DateCheck(DateTime startDate)
    {
        return startDate.Date >= DateTime.Today;
    }

    //Cheks if the enddate is not before the startdate
    public bool StartEndCheck(DateTime startDate, DateTime endDate)
    {
        return startDate.Date <= endDate.Date;
    }


    //Adds filters so that the user can filter their view.
    public async Task<IEnumerable<Listing>?> FilterListings(int? AmountGuests, int? AmountBathrooms, int? AmountBedrooms, int? MinPrice, int? MaxPrice)
    {
        try
        {
            //Includes all listings if no filters, as true is always true.
            var query = _db.Listings.Where(l => true);

            //If the guest filter is in use, a lamda expression only fetches listings that have that amount of guests or more.
            //All under uses the same logic
            if (AmountGuests.HasValue)
            {
                query = query.Where(l => l.Guests >= AmountGuests);
            }

            if (AmountBathrooms.HasValue)
            {
                query = query.Where(l => l.Bathrooms >= AmountBathrooms);
            }

            if (AmountBedrooms.HasValue)
            {
                query = query.Where(l => l.Bedrooms >= AmountBedrooms);
            }
            if (MinPrice.HasValue)
            {
                query = query.Where(l => l.Price >= MinPrice);
            }
            if (MaxPrice.HasValue)
            {
                query = query.Where(l => l.Price <= MaxPrice);
            }

            return await query.ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[ItemRepository] Failed to filter listings, error message: {e}", e.Message);
            return null;
        }
    }


    public async Task<IEnumerable<Reservation>> GetReservations()
    {
        try
        {
            return await _db.Reservations.ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[ItemRepository] reservations ToListAsync() failed when GetReservations(), error message: {e}", e.Message);
            return null;
        }
    }
    
    
    public async Task<IEnumerable<Reservation>?> GetReservationByPlace(string place)
    {
        try
        {
            return await _db.Reservations.Where(l => l.Place == place).ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[ItemRepository] Failed to find the reservation based on Place error message: {e}", e.Message);
            return null;
        }

    }



}