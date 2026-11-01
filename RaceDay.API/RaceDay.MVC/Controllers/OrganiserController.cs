using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers;

public class OrganiserController : Controller
{
    private readonly ApiClient _api;

    public OrganiserController(ApiClient api) => _api = api;

    public async Task<IActionResult> Dashboard()
    {
        // Checks if user is Organiser
        if (HttpContext.Session.GetString("Role") != "Organiser")
            return RedirectToAction("Index", "Home");

        // Fetch only events organised by this user
        var events = await _api.GetAsync<List<EventDto>>("api/events/my-events") ?? new List<EventDto>();
        return View(events);
    }
}
