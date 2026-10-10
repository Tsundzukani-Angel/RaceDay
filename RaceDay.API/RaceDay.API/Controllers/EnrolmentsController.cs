using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrolmentsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public EnrolmentsController(RaceDayDbContext db) => _db = db;

    /// <summary>Participant only: enter an event by choosing a category.</summary>
    /// <response code="201">Enrolment created.</response>
    /// <response code="400">Category does not belong to event.</response>
    /// <response code="403">Not a participant.</response>
    /// <response code="404">Category not found.</response>
    /// <response code="409">Already enrolled in this event.</response>
    [HttpPost]
    public async Task<IActionResult> Enrol(EnrolmentDto dto)
    {
        if (!HttpContext.Session.IsParticipant()) return Forbid();
        var userId = HttpContext.Session.GetUserId()!.Value;

        var cat = await _db.Categories.FindAsync(dto.CategoryID);
        if (cat == null) return NotFound(new { message = "Category not found." });
        if (cat.EventID != dto.EventID)
            return BadRequest(new { message = "Category does not belong to event." });

        var already = await _db.Enrolments
            .AnyAsync(e => e.ParticipantID == userId && e.EventID == dto.EventID);
        if (already) return Conflict(new { message = "Already enrolled in this event." });

        var enrolment = new Enrolment
        {
            ParticipantID = userId,
            EventID = dto.EventID,
            CategoryID = dto.CategoryID,
            PaymentStatus = "Pending"
        };
        _db.Enrolments.Add(enrolment);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(MyEnrolments), new { id = enrolment.EnrolmentID }, enrolment);
    }

    /// <summary>Participant only: list own enrolments with results.</summary>
    [HttpGet("my")]
    public async Task<IActionResult> MyEnrolments()
    {
        if (!HttpContext.Session.IsParticipant()) return Forbid();
        var userId = HttpContext.Session.GetUserId()!.Value;

        var list = await _db.Enrolments
            .Where(e => e.ParticipantID == userId)
            .Include(e => e.Event)
            .Include(e => e.Category)
            .Include(e => e.Result)
            .Select(e => new
            {
                e.EnrolmentID,
                e.PaymentStatus,
                e.RegistrationDate,
                e.RaceNumber,
                Event = new
                {
                    e.Event.EventID,
                    e.Event.Name,
                    e.Event.EventDate,
                    e.Event.Location,
                    e.Event.EventType
                },
                Category = new
                {
                    e.Category.CategoryID,
                    e.Category.CategoryName,
                    e.Category.EntryFee
                },
                Result = e.Result == null ? null : new
                {
                    e.Result.FinishTime,
                    e.Result.Position,
                    e.Result.Status
                }
            })
            .ToListAsync();

        return Ok(list);
    }

    /// <summary>Organiser only: view all enrolments for one of own events.</summary>
    [HttpGet("event/{eventId}")]
    public async Task<IActionResult> ByEvent(int eventId)
    {
        if (!HttpContext.Session.IsOrganiser()) return Forbid();
        var userId = HttpContext.Session.GetUserId()!.Value;

        var ev = await _db.Events.FindAsync(eventId);
        if (ev == null) return NotFound();
        if (ev.OrganiserID != userId) return Forbid();

        var list = await _db.Enrolments
            .Where(e => e.EventID == eventId)
            .Include(e => e.Participant)
            .Include(e => e.Category)
            .Select(e => new
            {
                e.EnrolmentID,
                e.PaymentStatus,
                e.RegistrationDate,
                e.RaceNumber,
                Participant = new
                {
                    e.Participant.UserID,
                    e.Participant.FirstName,
                    e.Participant.LastName,
                    e.Participant.Email
                },
                Category = e.Category.CategoryName
            })
            .ToListAsync();

        return Ok(list);
    }

    /// <summary>Organiser only: update payment status or assign race number.</summary>
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateEnrolmentStatusDto dto)
    {
        if (!HttpContext.Session.IsOrganiser()) return Forbid();
        var userId = HttpContext.Session.GetUserId()!.Value;

        var enrolment = await _db.Enrolments
            .Include(e => e.Event)
            .FirstOrDefaultAsync(e => e.EnrolmentID == id);
        if (enrolment == null) return NotFound();
        if (enrolment.Event.OrganiserID != userId) return Forbid();

        enrolment.PaymentStatus = dto.PaymentStatus;
        if (dto.RaceNumber != null) enrolment.RaceNumber = dto.RaceNumber;

        await _db.SaveChangesAsync();
        return Ok(enrolment);
    }
}