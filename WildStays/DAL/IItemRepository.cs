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

    Task<bool> Create(Listing listing, List<IFormFile> Images);
    Task<bool> ManageImages(Listing listing, List<IFormFile> newImages, int? imageToDeleteId);
    Task<bool> Update(Listing listing);
    Task<bool> Delete(int id);
    Task<bool> CreateReservation(Reservation reservation);
    bool DateCheck(DateTime startDate);
    bool StartEndCheck(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Listing>> FilterListings(int? AmountGuests, int? AmountBathrooms, int? AmountBedrooms,int? MinPrice, int? MaxPrice );

    Task<IEnumerable<Reservation>> GetReservations();
    
    
}


