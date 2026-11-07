using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models;

public class EventViewModel
{
    public int EventID { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime EventDate { get; set; } = DateTime.Now.AddDays(30);

    [Required, MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    [Required, Range(0.1, 1000)]
    public double Distance { get; set; }

    [Required]
    public string EventType { get; set; } = "Run";

    public int MaxParticipants { get; set; } = 500;

    public DateTime? RegistrationDeadline { get; set; } = DateTime.Now.AddDays(20);

    public string? BannerUrl { get; set; }

    // For uploading the image
    public IFormFile? BannerFile { get; set; }
}