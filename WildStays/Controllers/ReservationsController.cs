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
        private readonly DatabaseDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReservationsController(DatabaseDbContext context, UserManager<IdentityUser> usermanager)
        {
            _context = context;
            _userManager = usermanager;
        }

        // Action to display a list of available listings
        public IActionResult Index()
        {
            var listings = _context.Listings.ToList();
            return View(listings);
        }

        // Action to display details of a listing and allow reservation
        public IActionResult Details(int id)
        {
            var listing = _context.Listings.FirstOrDefault(l => l.Id == id);
            if (listing == null)
            {
                return NotFound();
            }

            return View(listing);
        }

        // Action to create a reservation
        [Authorize]
        [HttpPost]
        public IActionResult CreateReservation(int listingId, DateTime startDate, DateTime endDate)
        {
            bool isReservationSuccessful = false;

            try
            {
                var listing = _context.Listings.FirstOrDefault(l => l.Id == listingId);
                if (listing == null)
                {
                    return NotFound();
                }

                // Check if the listing is available for the selected date range
                bool isAvailable = !_context.Reservations.Any(r =>
                    r.ListingId == listingId &&
                    ((startDate >= r.StartDate && startDate <= r.EndDate) ||
                     (endDate >= r.StartDate && endDate <= r.EndDate)));

                if (isAvailable)
                {
                    // Get the authenticated user's ID
                    var userId = _userManager.GetUserId(User);

                    // Create a new reservation and save it to the database
                    var reservation = new Reservation
                    {
                        ListingId = listingId,
                        StartDate = startDate,
                        EndDate = endDate,
                        UserId = userId // Set the UserId to the authenticated user's ID
                    };

                    _context.Reservations.Add(reservation);
                    _context.SaveChanges();

                    // Set the flag to indicate a successful reservation
                    isReservationSuccessful = true;
                }
                else
                {
                    // Handle case where the listing is not available
                    ModelState.AddModelError(string.Empty, "This listing is not available for the selected dates.");
                }
            }
            catch (DbUpdateException ex)
            {
                // Log the exception for debugging purposes
                // You can use your preferred logging mechanism here
                Console.WriteLine("DbUpdateException occurred: " + ex.InnerException?.Message);
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
