using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs;

public class EventDto
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime EventDate { get; set; }

    [Required, MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    [Required, Range(0.1, 1000)]
    public double Distance { get; set; }

    [Required]
    public string EventType { get; set; } = "Run"; // "Run", "Walk" & "Cycle"

    public int MaxParticipants { get; set; } = 500;

    public DateTime? RegistrationDeadline { get; set; }

    public string? Status { get; set; } // "Open", "Closed" & "Completed"
}
