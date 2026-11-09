using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;
using System.Text.Json;

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

        // 1. If an image was uploaded, send it to the API first
        if (model.BannerFile != null && model.BannerFile.Length > 0)
        {
            var fileContent = new MultipartFormDataContent();
            var fileStream = model.BannerFile.OpenReadStream();
            fileContent.Add(new StreamContent(fileStream), "file", model.BannerFile.FileName);

            var uploadResponse = await _api.PostFileAsync("api/events/upload-banner", fileContent);

            if (uploadResponse.IsSuccessStatusCode)
            {
                var json = await uploadResponse.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                model.BannerUrl = doc.RootElement.GetProperty("bannerUrl").GetString();
            }
            else
            {
                ModelState.AddModelError("", "Failed to upload banner image.");
                return View(model);
            }
        }

        // 2. Map the ViewModel to the API DTO
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
            Status = "Open",
            BannerUrl = model.BannerUrl // Include the new URL
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

    [HttpGet]
    public async Task<IActionResult> Categories(int eventId)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser")
            return RedirectToAction("Index", "Home");

        var ev = await _api.GetAsync<EventDto>($"api/events/{eventId}");
        if (ev == null) return NotFound();

        var categories = await _api.GetAsync<List<CategoryDto>>($"api/categories/event/{eventId}") ?? new List<CategoryDto>();

        ViewBag.EventName = ev.Name;
        ViewBag.EventId = ev.EventID;

        return View(categories);
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory(CategoryViewModel model)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser")
            return RedirectToAction("Index", "Home");

        if (ModelState.IsValid)
        {
            var response = await _api.PostAsync("api/categories", model);

            if (response.IsSuccessStatusCode)
            {
                // If it worked, redirect back to the categories list
                return RedirectToAction(nameof(Categories), new { eventId = model.EventID });
            }

            // IF IT FAILED: Read the API's error and pass it to the view
            var errorJson = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"API Error: {errorJson}";
        }
        else
        {
            TempData["ErrorMessage"] = "Invalid form data.";
        }

        // If we get here, it failed. Redirect back to the page.
        return RedirectToAction(nameof(Categories), new { eventId = model.EventID });
    }

    [HttpGet]
    public async Task<IActionResult> Enrolments(int eventId)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser")
            return RedirectToAction("Index", "Home");

        var ev = await _api.GetAsync<EventDto>($"api/events/{eventId}");
        if (ev == null) return NotFound();

        var enrolments = await _api.GetAsync<List<OrganiserEnrolmentDto>>($"api/enrolments/event/{eventId}") ?? new List<OrganiserEnrolmentDto>();

        ViewBag.EventName = ev.Name;
        ViewBag.EventId = ev.EventID;

        return View(enrolments);
    }

    [HttpPost]
    public async Task<IActionResult> CaptureResult(int enrolmentId, string finishTime, int position, int eventId)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser")
            return RedirectToAction("Index", "Home");

        var resultDto = new
        {
            EnrolmentID = enrolmentId,
            FinishTime = TimeSpan.Parse(finishTime),
            Position = position,
            Status = "Finished"
        };

        var response = await _api.PostAsync("api/results", resultDto);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"Failed to capture result: {error}";
        }
        else
        {
            TempData["SuccessMessage"] = "Result captured successfully!";
        }

        return RedirectToAction(nameof(Enrolments), new { eventId = eventId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser")
            return RedirectToAction("Index", "Home");

        var ev = await _api.GetAsync<EventDto>($"api/events/{id}");
        if (ev == null) return NotFound();

        // Map the DTO back to the ViewModel
        var model = new EventViewModel
        {
            EventID = ev.EventID,
            Name = ev.Name,
            Description = ev.Description,
            EventDate = ev.EventDate,
            Location = ev.Location,
            Distance = ev.Distance,
            EventType = ev.EventType,
            MaxParticipants = ev.MaxParticipants,
            RegistrationDeadline = ev.RegistrationDeadline,
            BannerUrl = ev.BannerUrl
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EventViewModel model)
    {
        if (HttpContext.Session.GetString("Role") != "Organiser")
            return RedirectToAction("Index", "Home");

        if (!ModelState.IsValid) return View(model);

        if (model.BannerFile != null && model.BannerFile.Length > 0)
        {
            var fileContent = new MultipartFormDataContent();
            var fileStream = model.BannerFile.OpenReadStream();
            fileContent.Add(new StreamContent(fileStream), "file", model.BannerFile.FileName);

            var uploadResponse = await _api.PostFileAsync("api/events/upload-banner", fileContent);

            if (uploadResponse.IsSuccessStatusCode)
            {
                var json = await uploadResponse.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                model.BannerUrl = doc.RootElement.GetProperty("bannerUrl").GetString();
            }
            else
            {
                var uploadError = await uploadResponse.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Banner Upload Failed ({uploadResponse.StatusCode}): {uploadError}");
                return View(model); // STOP and show the error
            }
        }

        var eventDto = new EventDto
        {
            EventID = model.EventID,
            Name = model.Name,
            Description = model.Description,
            EventDate = model.EventDate,
            Location = model.Location,
            Distance = model.Distance,
            EventType = model.EventType,
            MaxParticipants = model.MaxParticipants,
            RegistrationDeadline = model.RegistrationDeadline,
            Status = "Open",
            BannerUrl = model.BannerUrl
        };

        // Debugging: Print to console
        Console.WriteLine($"SENDING PUT to api/events/{model.EventID}");

        var response = await _api.PutAsync($"api/events/{model.EventID}", eventDto);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Dashboard));
        }

        // Debugging: Show the exact error
        var errorJson = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"API ERROR: {response.StatusCode} - {errorJson}");

        ModelState.AddModelError("", $"API Error ({response.StatusCode}): {errorJson}");
        return View(model);
    }
}
