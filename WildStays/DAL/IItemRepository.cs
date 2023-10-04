using WildStays.Models;

namespace WildStays.DAL;

public interface IItemRepository
{
    Task<IEnumerable<Listing>> GetAll();
    Task<Listing?> GetItemById(int id);
    Task<IEnumerable<Listing>> GetListingsByUserId(string userId);
    Task<IEnumerable<Reservation>> GetReservationByUserId(string userId);
    Task<bool> Create(Listing listing);
    Task<bool> Update(Listing listing);
    Task<bool> Delete(int id);
    Task<bool> CreateReservation(Reservation reservation);
    bool DateCheck(DateTime startDate);
    bool StartEndCheck(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Listing>> FilterListings(int? minGuests, int? minBathrooms, int? minBedrooms);

}


