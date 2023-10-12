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

namespace WildStays.Controllers
{
    public class ListingsController : Controller
    {
        private readonly IItemRepository _itemRepository;
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
            if (user == null)
            {
                return RedirectToRoute("Identity/Account/Login");
            }

            var listings = await _itemRepository.GetListingsByUserId(user.Id);
            return View(listings);
        }

        // GET: Listings/Details/5
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
        public async Task<IActionResult> Create([Bind("Id,Name,Place,Description,Type,Price,Guests,Bedrooms,Bathrooms,Image,UserId,StartDate,EndDate")] Listing listing)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToRoute("Identity/Account/Login");
                }

                listing.UserId = user.Id;

                // validates that the startdate is after todays date
                if (!_itemRepository.DateCheck(listing.StartDate))
                {
                    ModelState.AddModelError("StartDate", "Start Date cannot be before today's date.");
                    return View(listing);
                }
                //Checks if the enddate is before the startdate
                if (!_itemRepository.StartEndCheck(listing.StartDate, listing.EndDate))
                {
                    ModelState.AddModelError("EndDate", "End Date cannot be before Start Date.");
                    return View(listing);
                }

                bool returnOk = await _itemRepository.Create(listing);

                if (returnOk)
                {
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Place,Description,Type,Price,Guests,Bedrooms,Bathrooms,Image,UserId,StartDate,EndDate")] Listing listing)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    var existingListing = await _itemRepository.GetItemById(listing.Id);

                    if (existingListing == null || existingListing.UserId != user.Id)
                    {
                        return Forbid();
                    }
                    listing.UserId = user.Id;

                    // Validates that the startdate is not before the current date
                    if (!_itemRepository.DateCheck(listing.StartDate))
                    {
                        ModelState.AddModelError("StartDate", "Start Date cannot be before today's date.");
                        return View(listing);
                    }

                    // Checks if the enddate is before the startdate
                    if (!_itemRepository.StartEndCheck(listing.StartDate, listing.EndDate))
                    {
                        ModelState.AddModelError("EndDate", "End Date cannot be before Start Date.");
                        return View(listing);
                    }

                    bool returnOk = await _itemRepository.Update(listing);

                    if (returnOk)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        // Handle update failure
                        TempData["ErrorMessage"] = "Failed to update the listing. Please try again later.";
                    }
                }
                // If ModelState is not valid, return the view with validation errors
                return View(listing);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while editing the listing: {ex}", ex);
                TempData["ErrorMessage"] = "An error occurred while editing the listing. Please try again later.";
                return RedirectToAction("Details", new { id = id });
            }
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

            if (listing == null)
            {
                return NotFound();
            }

            if (listing.UserId != user.Id)
            {
                return Forbid();
            }

            bool returnOk = await _itemRepository.Delete(id);

            if (!returnOk)
            {
                _logger.LogError("[ListingsController] Listing deletion failed for the Id {Id:0000}", id);
                return BadRequest("Listing deletion failed");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
