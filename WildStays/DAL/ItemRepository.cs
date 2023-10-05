using Microsoft.EntityFrameworkCore;
using WildStays.Models;

namespace WildStays.DAL;

public class ItemRepository : IItemRepository
{
    private readonly DatabaseDbContext _db;

    private readonly ILogger<ItemRepository> _logger;

    public ItemRepository(DatabaseDbContext db, ILogger<ItemRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

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


    public async Task<bool> Create(Listing listing)
    {
        try
        {
            _db.Listings.Add(listing);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[ItemRepository] failed to create the listing {@listing}, error message: {e}", listing, e.Message);
            return false;
        }
    }

    public async Task<bool> Update(Listing listing)
    {
        try
        {
            // Find the existing entity in the context and detach it if it's being tracked
            var existingListing = await _db.Listings.FindAsync(listing.Id);
            if (existingListing != null)
            {
                _db.Entry(existingListing).State = EntityState.Detached;
            }

            // Attach and update the entity
            _db.Entry(listing).State = EntityState.Modified;

            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[ItemRepository] failed to update the listing {@listing}, error message: {e}", listing, e.Message);
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

    public async Task<IEnumerable<Listing>?> GetListingsByUserId(string userId)
    {
        try
        {
            return await _db.Listings.Where(l => l.UserId == userId).ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[ItemRepository] Failed to get the listings based on the spescific user(Id), error message: {e}", e.Message);
            return null;
        }
    }
    public async Task<IEnumerable<Reservation>?> GetReservationByUserId(string userId)
    {
        try
        {
            return await _db.Reservations.Where(l => l.UserId == userId).Include(r => r.Listing).ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[ItemRepository] Failed to find the reservation based on UserID error message: {e}", e.Message);
            return null;
        }

    }



    public async Task<bool> CreateReservation(Reservation reservation)
    {
        try
        {
            // Check if the listing is available for the dates
            bool isAvailable = !_db.Reservations.Any(r =>
                r.ListingId == reservation.ListingId &&
                ((reservation.StartDate >= r.StartDate && reservation.StartDate <= r.EndDate) ||
                 (reservation.EndDate >= r.StartDate && reservation.EndDate <= r.EndDate)));

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
    public bool DateCheck(DateTime startDate)
    {
        return startDate.Date >= DateTime.Today;
    }

    public bool StartEndCheck(DateTime startDate, DateTime endDate)
    {
        return startDate.Date <= endDate.Date;
    }

    // ItemRepository
    // ItemRepository
    // ItemRepository
    public async Task<IEnumerable<Listing>?> FilterListings(int? AmountGuests, int? AmountBathrooms, int? AmountBedrooms, int? MinPrice, int? MaxPrice)
    {
        try
        {
            //Includes all listings if no filters, as true is always true
            var query = _db.Listings.Where(l => true); 

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






}

