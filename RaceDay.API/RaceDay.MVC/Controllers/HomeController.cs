using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers;

public class HomeController : Controller
{
    private readonly ApiClient _api;

    public HomeController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        // Fetch events from the API
        var events = await _api.GetAsync<List<EventDto>>("api/events") ?? new List<EventDto>();
        return View(events);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public async Task<IActionResult> EventDetails(int id)
    {
        var ev = await _api.GetAsync<EventDto>($"api/events/{id}");
        if (ev == null) return NotFound();

        var model = new EventDetailsViewModel
        {
            Event = ev,
            IsParticipant = HttpContext.Session.GetString("Role") == "Participant"
        };

        // Check if this participant is already enrolled
        if (model.IsParticipant)
        {
            var myEnrolments = await _api.GetAsync<List<EnrolmentDto>>("api/enrolments/my");
            if (myEnrolments != null && myEnrolments.Any(e => e.Event.EventID == id))
            {
                model.AlreadyEnrolled = true;
            }
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Enrol(int eventId, int categoryId)
    {
        if (HttpContext.Session.GetString("Role") != "Participant")
            return RedirectToAction("Login", "Account");

        var enrolDto = new { EventID = eventId, CategoryID = categoryId };
        var response = await _api.PostAsync("api/enrolments", enrolDto);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "You have successfully enrolled!";
            return RedirectToAction("MyEvents");
        }

        var error = await response.Content.ReadAsStringAsync();
        TempData["ErrorMessage"] = $"Failed to enrol: {error}";
        return RedirectToAction(nameof(EventDetails), new { id = eventId });
    }

    [HttpGet]
    public async Task<IActionResult> MyEvents()
    {
        if (HttpContext.Session.GetString("Role") != "Participant")
            return RedirectToAction("Index", "Home");

        var enrolments = await _api.GetAsync<List<EnrolmentDto>>("api/enrolments/my") ?? new List<EnrolmentDto>();
        return View(enrolments);
    }

}
