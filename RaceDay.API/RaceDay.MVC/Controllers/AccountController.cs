using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers;

public class AccountController : Controller
{
    private readonly ApiClient _api;

    public AccountController(ApiClient api) => _api = api;

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetString("UserId") != null)
            return RedirectToAction("Index", "Home");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _api.LoginAsync(model);
        if (user == null)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        // Store user info in MVC session
        HttpContext.Session.SetString("UserId", user.UserID.ToString());
        HttpContext.Session.SetString("Role", user.Role);
        HttpContext.Session.SetString("FullName", $"{user.FirstName} {user.LastName}");

        if (user.Role == "Organiser")
            return RedirectToAction("Dashboard", "Organiser");

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var response = await _api.PostAsync("api/auth/register", model);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Registration successful! Please log in.";
            return RedirectToAction(nameof(Login));
        }

        // Extract the exact error from the API
        var errorJson = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(errorJson))
        {
            ModelState.AddModelError("", $"API Error: {response.StatusCode}");
        }
        else
        {
            // Show the raw error for anything else (400, 500, etc)
            ModelState.AddModelError("", $"API Error ({response.StatusCode}): {errorJson}");
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _api.PostAsync("/auth/logout", new { });
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        if (HttpContext.Session.GetString("UserId") == null)
            return RedirectToAction("Login");

        var user = await _api.GetAsync<UserResponse>("api/users/profile");
        if (user == null) return NotFound();

        // Get full profile details (the basic UserResponse doesn't include Profile)
        var model = new ProfileViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            PhoneNumber = user.PhoneNumber
            };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        if (HttpContext.Session.GetString("UserId") == null)
            return RedirectToAction("Login");

        var updateDto = new UpdateProfileDto
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber
        };

        var response = await _api.PutAsync("api/users/profile", updateDto);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Profile updated successfully!";
            // Update the navbar name
            HttpContext.Session.SetString("FullName", $"{model.FirstName} {model.LastName}");
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to update profile.";
        }

        return RedirectToAction(nameof(Profile));
    }
}
