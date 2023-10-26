using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WildStays.DAL;
using WildStays.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;


namespace WildStays.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly IItemRepository _itemRepository;
        private readonly ILogger<ListingsController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationsController(IItemRepository itemRepository,
            ILogger<ListingsController> logger,
            UserManager<ApplicationUser> userManager)
        {
            _itemRepository = itemRepository;
            _logger = logger;
            _userManager = userManager;
        }



        // Index
        public async Task<IActionResult> Index(String? Place, int? AmountGuests, int? AmountBathrooms, int? AmountBedrooms, int? MinPrice, int? MaxPrice, DateTime? StartDate, DateTime? EndDate)
        {
            //Returnes the listings with filters, if any, see the method in Itemrepository for further information.
            var listings = await _itemRepository.FilterListings(Place, AmountGuests, AmountBathrooms, AmountBedrooms, MinPrice, MaxPrice, StartDate, EndDate);

            return View(listings);
        }


        // Detail action, same as in listingscontroller, but has reservations in it
        public async Task<IActionResult> Details(int id)
     {
         var listing = await _itemRepository.GetItemById(id);
         if (listing == null)
         {
             return NotFound();
         }

         var reservations = await _itemRepository.GetReservationsByListingId(id);

         var viewModel = new ListingDetailsViewModel
         {
             Listing = listing,
             Reservations = reservations
         };

         return View(viewModel);
     }

        

        // Creates a reservation
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateReservation(int listingId, DateTime startDate, DateTime endDate, string Place)
        {
            try
            {
                var listing = await _itemRepository.GetItemById(listingId);
                if (listing == null)
                {
                    return NotFound();
                }

                if (!_itemRepository.DateCheck(startDate))
                {
                    TempData["ErrorMessage"] = "Start date must be after today's date.";
                    return RedirectToAction("Details", new { id = listingId });
                }

                if (!_itemRepository.StartEndCheck(startDate, endDate))
                {
                    TempData["ErrorMessage"] = "End date must be after the start date.";
                    return RedirectToAction("Details", new { id = listingId });
                }

                var userId = _userManager.GetUserId(User);
                var reservation = new Reservation
                {
                    ListingId = listingId,
                    StartDate = startDate,
                    EndDate = endDate,
                    UserId = userId,
                    Place = listing.Place  // Set the Place from the Listing to the Reservation
                };

                bool isReservationSuccessful = await _itemRepository.CreateReservation(reservation);
                if (isReservationSuccessful)
                {
                    return View("ReservationConfirmation", reservation);
                }
                else
                {
                    TempData["ErrorMessage"] = "This listing is not available for the selected dates.";
                    return RedirectToAction("Details", new { id = listingId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while creating the reservation: {ex}", ex);
                TempData["ErrorMessage"] = "An error occurred while creating the reservation. Please try again later.";
                return RedirectToAction("Details", new { id = listingId });
            }
        }



    }
}
