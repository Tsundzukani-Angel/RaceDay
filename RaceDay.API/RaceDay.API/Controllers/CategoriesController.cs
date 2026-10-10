using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public CategoriesController(RaceDayDbContext db) => _db = db;

    /// <summary>Public: list categories for a specific event.</summary>
    [HttpGet("event/{eventId}")]
    public async Task<IActionResult> ByEvent(int eventId)
        => Ok(await _db.Categories.Where(c => c.EventID == eventId).ToListAsync());

    /// <summary>Organiser only: add a category to own event.</summary>
    /// <response code="200">Category created.</response>
    /// <response code="403">Not the event organiser.</response>
    /// <response code="404">Event not found.</response>
    [HttpPost]
    public async Task<IActionResult> Create(CategoryDto dto)
    {
        if (!HttpContext.Session.IsOrganiser()) return Forbid();
        var userId = HttpContext.Session.GetUserId()!.Value;

        var ev = await _db.Events.FindAsync(dto.EventID);
        if (ev == null) return NotFound(new { message = "Event not found." });
        if (ev.OrganiserID != userId) return Forbid();

        var cat = new Category
        {
            EventID = dto.EventID,
            CategoryName = dto.CategoryName,
            CategoryType = dto.CategoryType,
            EntryFee = dto.EntryFee
        };
        _db.Categories.Add(cat);
        await _db.SaveChangesAsync();
        return Ok(cat);
    }

    /// <summary>Organiser only: delete own category.</summary>
    /// <response code="204">Category deleted.</response>
    /// <response code="403">Not the event organiser.</response>
    /// <response code="404">Category not found.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!HttpContext.Session.IsOrganiser()) return Forbid();
        var userId = HttpContext.Session.GetUserId()!.Value;

        var cat = await _db.Categories
            .Include(c => c.Event)
            .FirstOrDefaultAsync(c => c.CategoryID == id);

        if (cat == null) return NotFound();
        if (cat.Event.OrganiserID != userId) return Forbid();

        _db.Categories.Remove(cat);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}