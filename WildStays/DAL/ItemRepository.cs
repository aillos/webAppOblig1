using Microsoft.EntityFrameworkCore;
using WildStays.Models;

namespace WildStays.DAL;

public class ItemRepository : IItemRepository
{
    private readonly DatabaseContext _db;

    private readonly ILogger<ItemRepository> _logger;

    public ItemRepository(DatabaseContext db, ILogger<ItemRepository> logger)
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
            _logger.LogError("[ItemRepository] items ToListAsync() failed when GetAll(), error message: {e}", e.Message);
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
            _logger.LogError("[ItemRepository] item FindAsync(id) failed when GetItemById for ItemId {ItemId:0000}, error message: {e}", id, e.Message);
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
            _logger.LogError("[ItemRepository] listing creation failed for listing {@listing}, error message: {e}", listing, e.Message);
            return false;
        }
    }

    public async Task<bool> Update(Listing listing)
    {
        try
        {
            _db.Listings.Update(listing);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[ItemRepository] listing creation failed for listing {@listing}, error message: {e}", listing, e.Message);
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
                _logger.LogError("[ItemRepository] item not found for the ItemId {ItemId:0000}", id);
                return false;
            }

            _db.Listings.Remove(listing);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[ItemRepository] item deletion failed for the ItemId {ItemId:0000}, error message: {e}", id, e.Message);
            return false;
        }
    }

    public async Task<IEnumerable<Listing>> GetListingsByUserId(string userId)
    {
        try
        {
            return await _db.Listings.Where(l => l.UserId == userId).ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[ItemRepository] Failed to fetch listings by user ID, error message: {e}", e.Message);
            return null;
        }
    }

}

