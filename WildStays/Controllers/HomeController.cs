using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WildStays.DAL;
using WildStays.Models;

namespace WildStays.Controllers;

public class HomeController : Controller
{

    private readonly IItemRepository _itemRepository;
    private readonly ILogger<ReservationsController> _logger;
    private readonly UserManager<IdentityUser> _userManager;

    public HomeController(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<IActionResult> Index()
    {
        var reservations = await _itemRepository.GetReservations();
        ViewData["SearchTerm"] = HttpContext.Request.Query["Search"];
        return View(reservations);
    }
    
    

}