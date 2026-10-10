using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    private readonly IPasswordHasher _hasher;

    public AuthController(RaceDayDbContext db, IPasswordHasher hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    /// <summary>Register a new user as Organiser or Participant.</summary>
    /// <response code="201">User created.</response>
    /// <response code="400">Invalid role or payload.</response>
    /// <response code="409">Email already registered.</response>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (dto.Role != "Organiser" && dto.Role != "Participant")
            return BadRequest(new { message = "Role must be 'Organiser' or 'Participant'." });

        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            return Conflict(new { message = "Email already registered." });

        var role = await _db.Roles.FirstOrDefaultAsync(r => r.RoleName == dto.Role);
        if (role == null) return BadRequest(new { message = "Role not found." });

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = _hasher.Hash(dto.Password),
            PhoneNumber = dto.PhoneNumber,
            RoleID = role.RoleID
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Me), new { id = user.UserID }, new
        {
            user.UserID,
            user.FirstName,
            user.LastName,
            user.Email,
            Role = role.RoleName
        });
    }

    /// <summary>Login and create a session.</summary>
    /// <response code="200">Login successful, session cookie set.</response>
    /// <response code="401">Invalid credentials.</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !_hasher.Verify(dto.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password." });

        HttpContext.Session.SetInt32(RaceDaySession.UserIdKey, user.UserID);
        HttpContext.Session.SetString(RaceDaySession.RoleKey, user.Role.RoleName);

        return Ok(new
        {
            user.UserID,
            user.FirstName,
            user.LastName,
            user.Email,
            Role = user.Role.RoleName
        });
    }

    /// <summary>Logout: clear session.</summary>
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Ok(new { message = "Logged out." });
    }

    /// <summary>Get the currently logged-in user.</summary>
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var id = HttpContext.Session.GetUserId();
        if (id == null) return Unauthorized();

        var user = await _db.Users
            .Include(u => u.Role)
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.UserID == id);

        if (user == null) return Unauthorized();

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
}
