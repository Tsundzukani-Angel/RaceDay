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

    [HttpGet]
    public IActionResult Create()
    {
        if (HttpContext.Session.GetString("Role") != "Organiser")
            return RedirectToAction("Index", "Home");

        return View(new EventViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(EventViewModel model)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser")
            return RedirectToAction("Index", "Home");

        if (!ModelState.IsValid) return View(model);

        // Map the ViewModel to the API DTO
        var eventDto = new EventDto
        {
            Name = model.Name,
            Description = model.Description,
            EventDate = model.EventDate,
            Location = model.Location,
            Distance = model.Distance,
            EventType = model.EventType,
            MaxParticipants = model.MaxParticipants,
            RegistrationDeadline = model.RegistrationDeadline,
            Status = "Open"
        };

        var response = await _api.PostAsync("api/events", eventDto);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Dashboard));
        }

        var errorJson = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", $"API Error: {errorJson}");
       
        return View(model);
    }
}
