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


        // Action to display a list of available listings
        public async Task<IActionResult> Index()
        {
            var listings = await _itemRepository.GetAll();
            return View(listings);
        }

        // Action to display details of a listing and allow reservation
        public async Task<IActionResult> Details(int id)
        {
            var listing = await _itemRepository.GetItemById(id);
            if (listing == null)
            {
                return NotFound();
            }
            return View(listing);
        }

        // Action to create a reservation
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateReservation(int listingId, DateTime startDate, DateTime endDate)
        {
            bool isReservationSuccessful = false;

            try
            {
                var listing = await _itemRepository.GetItemById(listingId);
                if (listing == null)
                {
                    return NotFound();
                }

                // Get the authenticated user's ID
                var userId = _userManager.GetUserId(User);

                // Create a new reservation
                var reservation = new Reservation
                {
                    ListingId = listingId,
                    StartDate = startDate,
                    EndDate = endDate,
                    UserId = userId
                };

                // Try to create the reservation
                if (await _itemRepository.CreateReservation(reservation))
                {
                    isReservationSuccessful = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "This listing is not available for the selected dates.");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                _logger.LogError("An error occurred while creating the reservation: {ex}", ex);
                ModelState.AddModelError(string.Empty, "An error occurred while creating the reservation. Please try again later.");
            }

            if (isReservationSuccessful)
            {
                // Redirect to a confirmation page or other appropriate action
                return RedirectToAction("ReservationConfirmation");
            }
            else
            {
                // Return to the Details view with error messages
                return RedirectToAction("Details", new { id = listingId });
            }
        }

    }
}
