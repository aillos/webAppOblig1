using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WildStays.Models;

namespace WildStays.Controllers;

public class ListingsController : Controller
{

    private readonly DatabaseContext _databaseContext;

    public ListingsController(DatabaseContext db)
    {
        _databaseContext = db;
    }

    public IActionResult Listings()
    {
        List<Listing> listings = _databaseContext.Listings.ToList();
        return View(listings);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Listing listing)
    {
        if (ModelState.IsValid)
        {
            _databaseContext.Listings.Add(listing);
            _databaseContext.SaveChanges();
            return RedirectToAction(nameof(Create));
          
        }
        return View(listing);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

