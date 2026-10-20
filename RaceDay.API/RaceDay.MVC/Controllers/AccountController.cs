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

        var response = await _api.PostAsync("/auth/register", model);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Registration successful! Please log in.";
            return RedirectToAction(nameof(Login));
        }

        ModelState.AddModelError("", "Registration failed. Email might already exist.");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _api.PostAsync("/auth/logout", new { });
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
