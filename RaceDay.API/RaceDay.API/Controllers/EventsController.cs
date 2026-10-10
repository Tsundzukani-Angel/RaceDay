using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public EventsController(RaceDayDbContext db) => _db = db;

    /// <summary>Public: list all events with categories.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var events = await _db.Events
            .Include(e => e.Categories)
            .Include(e => e.Organiser)
            .OrderBy(e => e.EventDate)
            .Select(e => new
            {
                e.EventID,
                e.Name,
                e.Description,
                e.EventDate,
                e.Location,
                e.Distance,
                e.EventType,
                e.MaxParticipants,
                e.RegistrationDeadline,
                e.Status,
                e.BannerUrl,
                Organiser = new { e.Organiser.UserID, e.Organiser.FirstName, e.Organiser.LastName },
                Categories = e.Categories.Select(c => new { c.CategoryID, c.CategoryName, c.CategoryType, c.EntryFee })
            })
            .ToListAsync();
        return Ok(events);
    }

    /// <summary>Public: get event details by ID.</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var ev = await _db.Events
            .Include(e => e.Categories)
            .Include(e => e.Organiser)
            .FirstOrDefaultAsync(e => e.EventID == id);
        if (ev == null) return NotFound();
        return Ok(ev);
    }

    /// <summary>Organiser only: create event.</summary>
    /// <response code="201">Event created.</response>
    /// <response code="403">Not an organiser.</response>
    [HttpPost]
    public async Task<IActionResult> Create(EventDto dto)
    {
        if (!HttpContext.Session.IsOrganiser()) return Forbid();
        var organiserId = HttpContext.Session.GetUserId()!.Value;

        var ev = new Event
        {
            Name = dto.Name,
            Description = dto.Description,
            EventDate = dto.EventDate,
            Location = dto.Location,
            Distance = dto.Distance,
            EventType = dto.EventType,
            MaxParticipants = dto.MaxParticipants,
            RegistrationDeadline = dto.RegistrationDeadline,
            Status = dto.Status ?? "Open",
            OrganiserID = organiserId
        };
        _db.Events.Add(ev);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = ev.EventID }, ev);
    }

    /// <summary>Organiser only: update own event.</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, EventDto dto)
    {
        if (!HttpContext.Session.IsOrganiser()) return Forbid();
        var userId = HttpContext.Session.GetUserId()!.Value;

        var ev = await _db.Events.FindAsync(id);
        if (ev == null) return NotFound();
        if (ev.OrganiserID != userId) return Forbid();

        ev.Name = dto.Name;
        ev.Description = dto.Description;
        ev.EventDate = dto.EventDate;
        ev.Location = dto.Location;
        ev.Distance = dto.Distance;
        ev.EventType = dto.EventType;
        ev.MaxParticipants = dto.MaxParticipants;
        ev.RegistrationDeadline = dto.RegistrationDeadline;
        if (dto.Status != null) ev.Status = dto.Status;

        await _db.SaveChangesAsync();
        return Ok(ev);
    }

    /// <summary>Organiser only: delete own event.</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!HttpContext.Session.IsOrganiser()) return Forbid();
        var userId = HttpContext.Session.GetUserId()!.Value;

        var ev = await _db.Events.FindAsync(id);
        if (ev == null) return NotFound();
        if (ev.OrganiserID != userId) return Forbid();

        _db.Events.Remove(ev);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Organiser only: list own events with enrolment counts.</summary>
    [HttpGet("my-events")]
    public async Task<IActionResult> MyEvents()
    {
        if (!HttpContext.Session.IsOrganiser()) return Forbid();
        var userId = HttpContext.Session.GetUserId()!.Value;

        var events = await _db.Events
            .Where(e => e.OrganiserID == userId)
            .Include(e => e.Categories)
            .Include(e => e.Enrolments)
            .OrderBy(e => e.EventDate)
            .Select(e => new
            {
                e.EventID,
                e.Name,
                e.EventDate,
                e.Location,
                EnrolmentCount = e.Enrolments.Count,
                CategoryCount = e.Categories.Count,
                e.Status
            })
            .ToListAsync();
        return Ok(events);
    }
}
