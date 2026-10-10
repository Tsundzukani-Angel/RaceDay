using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public UsersController(RaceDayDbContext db) => _db = db;

    /// <summary>View own profile (authenticated users only).</summary>
    /// <response code="200">Profile returned.</response>
    /// <response code="401">Not logged in.</response>
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var id = HttpContext.Session.GetUserId();
        if (id == null) return Unauthorized();

        var user = await _db.Users
            .Include(u => u.Role)
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.UserID == id);

        if (user == null) return NotFound();

        return Ok(new
        {
            user.UserID,
            user.FirstName,
            user.LastName,
            user.Email,
            Role = user.Role.RoleName,
            user.PhoneNumber,
            Profile = user.Profile
        });
    }

    /// <summary>Update own profile (authenticated users only).</summary>
    /// <response code="200">Profile updated.</response>
    /// <response code="401">Not logged in.</response>
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
    {
        var id = HttpContext.Session.GetUserId();
        if (id == null) return Unauthorized();

        var user = await _db.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.UserID == id);

        if (user == null) return NotFound();

        if (dto.FirstName != null) user.FirstName = dto.FirstName;
        if (dto.LastName != null) user.LastName = dto.LastName;
        if (dto.PhoneNumber != null) user.PhoneNumber = dto.PhoneNumber;

        user.Profile ??= new Profile { UserID = user.UserID };
        if (dto.DateOfBirth.HasValue) user.Profile.DateOfBirth = dto.DateOfBirth;
        if (dto.Gender != null) user.Profile.Gender = dto.Gender;
        if (dto.EmergencyContact != null) user.Profile.EmergencyContact = dto.EmergencyContact;
        if (dto.EmergencyNotes != null) user.Profile.EmergencyNotes = dto.EmergencyNotes;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Profile updated." });
    }
}