﻿using WildStays.Models;

namespace WildStays.DAL;

public interface IItemRepository
{
    Task<IEnumerable<Listing>> GetAll();
    Task<Listing?> GetItemById(int id);
    Task<IEnumerable<Listing>> GetListingsByUserId(string userId);
    Task<IEnumerable<Reservation>> GetReservationByUserId(string userId);
    Task<IEnumerable<Reservation>> GetReservationByPlace(string place);
    Task<IEnumerable<Image>> GetImagesByListingId(int listingId);
    Task<bool> DeleteImage(int Id);

    Task<bool> Create(Listing listing, List<IFormFile> Images);
    Task<bool> Update(Listing existingListing, Listing updatedListing, List<IFormFile> newImages, int? imageToDeleteId);
    Task<bool> Delete(int id);
    Task<bool> CreateReservation(Reservation reservation);
    bool DateCheck(DateTime startDate);
    bool StartEndCheck(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Listing>> FilterListings(String? Place, int? AmountGuests, int? AmountBathrooms, int? AmountBedrooms, int? MinPrice, int? MaxPrice, DateTime? StartDate, DateTime? EndDate);

    Task<IEnumerable<Reservation>> GetReservations();
    Task<IEnumerable<Reservation>> GetReservationsByListingId(int listingId);
    
    
}


