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
        private readonly UserManager<IdentityUser> _userManager;

        public ReservationsController(IItemRepository itemRepository,
            ILogger<ListingsController> logger,
            UserManager<IdentityUser> userManager)
        {
            _itemRepository = itemRepository;
            _logger = logger;
            _userManager = userManager;
        }


        // Action to display listings
        public async Task<IActionResult> Index()
        {
            var listings = await _itemRepository.GetAll();
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
            return View(listing);
        }

        // Creates a reservation
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateReservation(int listingId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var listing = await _itemRepository.GetItemById(listingId);
                if (listing == null)
                {
                    return NotFound();
                }
                // Gets todays  date
                var today = DateTime.Today;
                // Check to make sure the user is not trying to reserve before todays date.
                if (startDate <= today)
                {
                    TempData["ErrorMessage"] = "Start date must be after today's date.";
                    return RedirectToAction("Details", new { id = listingId });
                }

                //Check if a user is trying to have a startdate that is after the enddate
                //And gives warning that the enddate must be after the startdate
                if(startDate >= endDate) {

                    TempData["ErrorMessage"] = "End date must be after start date" +
                        "";
                    return RedirectToAction("Details", new { id = listingId });

                }

                // Get the user id
                var userId = _userManager.GetUserId(User);

                // Create  new reservation
                var reservation = new Reservation
                {
                    ListingId = listingId,
                    StartDate = startDate,
                    EndDate = endDate,
                    UserId = userId
                };

                // Try to create the reservation
                bool isReservationSuccessful = await _itemRepository.CreateReservation(reservation);

                if (isReservationSuccessful)
                {
                    // retun the reservationconfirmation view with the reservation
                    return View("ReservationConfirmation", reservation);
                }
                else
                {
                    //If the listing is not available
                    TempData["ErrorMessage"] = "This listing is not available for the selected dates.";
                    return RedirectToAction("Details", new { id = listingId });
                }
            }
            catch (Exception ex)
            {
                // Handles exceptions and logs them
                _logger.LogError("An error occurred while creating the reservation: {ex}", ex);
                TempData["ErrorMessage"] = "An error occurred while creating the reservation. Please try again later.";
                return RedirectToAction("Details", new { id = listingId });
            }
        }

    }
}
