using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WildStays.DAL;
using WildStays.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Reflection;
using WildStays.Migrations;

namespace WildStays.Controllers
{
    public class ListingsController : Controller
    {
        private readonly IItemRepository _itemRepository;

        //Adds logging as in the modules.
        private readonly ILogger<ListingsController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public ListingsController(IItemRepository itemRepository,
            ILogger<ListingsController> logger,
            UserManager<ApplicationUser> userManager)
        {
            _itemRepository = itemRepository;
            _logger = logger;
            _userManager = userManager;
        }



        // GET: Listings
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            //If a user is not logged in
            if (user == null)
            {
                return RedirectToRoute("Identity/Account/Login");
            }
            //Gets all the listings for a user.
            var listings = await _itemRepository.GetListingsByUserId(user.Id);
            return View(listings);
        }

        // GET: Listings/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var listing = await _itemRepository.GetItemById(id);
            if (listing == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (listing.UserId != user.Id)
            {
                return Forbid();
            }

            return View(listing);
        }

        // GET: Listings/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Listings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Listing listing, List<IFormFile> Images)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToRoute("Identity/Account/Login");
                }

                // Check if the start date is after today's date
                if (!_itemRepository.DateCheck(listing.StartDate))
                {
                    ModelState.AddModelError("StartDate", "Start Date cannot be before today's date.");
                    return View(listing);
                }

                // Check if the end date is before the start date
                if (!_itemRepository.StartEndCheck(listing.StartDate, listing.EndDate))
                {
                    ModelState.AddModelError("EndDate", "End Date cannot be before Start Date.");
                    return View(listing);
                }

                //Set User Id
                listing.UserId = user.Id;
                // Create the listing
                bool returnOk = await _itemRepository.Create(listing, Images);
                if (returnOk)
                {
                    _logger.LogDebug("Image url", Images);
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(listing);
        }

        // GET: Listings/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var listing = await _itemRepository.GetItemById(id);
            if (listing == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (listing.UserId != user.Id)
            {
                return Forbid();
            }

            return View(listing);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, Listing listing, List<IFormFile> Images, int? deleteImage, string submit)
        {
            //If the listing is not found
            if (id != listing.Id)
            {
                _logger.LogWarning("Wrong ID: {ListingId}. Returning NotFound.", id);
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            //Gets the listings by the listing.id
            var existingListing = await _itemRepository.GetItemById(listing.Id);

            if (existingListing == null || existingListing.UserId != user.Id)
            {
                _logger.LogWarning("This listing is not available listing ID: {ListingId}. Returning Forbid.", id);
                return Forbid();
            }

            try
            {
                //If the delete image button is tapped
                if (deleteImage.HasValue)
                {
                    // Delete the image
                    bool imageDeleted = await _itemRepository.DeleteImage(deleteImage.Value);

                }

                if (ModelState.IsValid)
                {

                    // Update the UserId so that it matches.
                    listing.UserId = user.Id;

                    var result = await _itemRepository.Update(existingListing, listing, Images, null); // Pass null for imageToDeleteId

                    if (result)
                    {
                        _logger.LogInformation("Listing details and images updated successfully for listing ID: {ListingId}. Redirecting to Edit.", id);
                        return RedirectToAction("Edit", new { id = existingListing.Id });
                    }
                    else
                    {
                        _logger.LogError("Failed to update listing and images for listing ID: {ListingId}.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Edit for listing ID: {ListingId}.", id);
            }
            return View(existingListing);
        }





        // GET: Listings/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var listing = await _itemRepository.GetItemById(id);
            if (listing == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (listing.UserId != user.Id)
            {
                return Forbid();
            }

            return View(listing);
        }



        // POST: Listings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var listing = await _itemRepository.GetItemById(id);

            // Checks that listing exists, and the user is the same user that made the listing.
            if (listing == null)
            {
                return NotFound();
            }

            if (listing.UserId != user.Id)
            {
                return Forbid();
            }

            // Method to delete reservation
            bool returnOk = await _itemRepository.Delete(id);

            // If there was a problem deleting the reservation.
            if (!returnOk)
            {
                _logger.LogError("[ListingsController] Listing deletion failed for the Id {Id:0000}", id);
                return BadRequest("Listing deletion failed");
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Listings/MyReservations
        //Controller to show the user their reservations.
        [Authorize]
        public async Task<IActionResult> Reservation()
        {
            //Gets the user information
            var user = await _userManager.GetUserAsync(User);
            //If a user is not logged in they are returned to the login screen.
            if (user == null)
            {
                return RedirectToAction("Login", "Identity/Account");
            }
            //Gets reservations based on UserID
            var reservations = await _itemRepository.GetReservationByUserId(user.Id);
            return View(reservations);
        }

    }
}