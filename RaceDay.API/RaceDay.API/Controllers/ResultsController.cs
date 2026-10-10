using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResultsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public ResultsController(RaceDayDbContext db) => _db = db;

    /// <summary>Organiser only: capture a result for an enrolment in own event.</summary>
    /// <response code="200">Result captured.</response>
    /// <response code="403">Not the event organiser.</response>
    /// <response code="404">Enrolment not found.</response>
    /// <response code="409">Result already captured.</response>
    [HttpPost]
    public async Task<IActionResult> Capture(ResultDto dto)
    {
        if (!HttpContext.Session.IsOrganiser()) return Forbid();
        var userId = HttpContext.Session.GetUserId()!.Value;

        var enrolment = await _db.Enrolments
            .Include(e => e.Event)
            .FirstOrDefaultAsync(e => e.EnrolmentID == dto.EnrolmentID);

        if (enrolment == null) return NotFound(new { message = "Enrolment not found." });
        if (enrolment.Event.OrganiserID != userId) return Forbid();

        if (await _db.Results.AnyAsync(r => r.EnrolmentID == dto.EnrolmentID))
            return Conflict(new { message = "Result already captured for this enrolment." });

        var result = new Result
        {
            EnrolmentID = dto.EnrolmentID,
            FinishTime = dto.FinishTime,
            Position = dto.Position,
            Status = dto.Status
        };
        _db.Results.Add(result);
        await _db.SaveChangesAsync();
        return Ok(result);
    }

    /// <summary>Participant only: own race history with results.</summary>
    [HttpGet("my")]
    public async Task<IActionResult> MyResults()
    {
        if (!HttpContext.Session.IsParticipant()) return Forbid();
        var userId = HttpContext.Session.GetUserId()!.Value;

        var results = await _db.Results
            .Include(r => r.Enrolment).ThenInclude(e => e.Event)
            .Include(r => r.Enrolment).ThenInclude(e => e.Category)
            .Where(r => r.Enrolment.ParticipantID == userId)
            .Select(r => new
            {
                r.ResultID,
                EventName = r.Enrolment.Event.Name,
                EventDate = r.Enrolment.Event.EventDate,
                EventType = r.Enrolment.Event.EventType,
                Category = r.Enrolment.Category.CategoryName,
                r.FinishTime,
                r.Position,
                r.Status,
                r.RecordedDate
            })
            .ToListAsync();

        return Ok(results);
    }

    /// <summary>Public: all results for a specific event.</summary>
    [HttpGet("event/{eventId}")]
    public async Task<IActionResult> ByEvent(int eventId)
    {
        var list = await _db.Results
            .Include(r => r.Enrolment).ThenInclude(e => e.Participant)
            .Include(r => r.Enrolment).ThenInclude(e => e.Category)
            .Where(r => r.Enrolment.EventID == eventId)
            .OrderBy(r => r.Position)
            .Select(r => new
            {
                r.ResultID,
                Participant = new
                {
                    r.Enrolment.Participant.FirstName,
                    r.Enrolment.Participant.LastName
                },
                Category = r.Enrolment.Category.CategoryName,
                r.FinishTime,
                r.Position,
                r.Status
            })
            .ToListAsync();

        return Ok(list);
    }
}