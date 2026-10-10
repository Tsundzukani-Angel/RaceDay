using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs;

public class RegisterDto
{
    [Required, MaxLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "Participant"; // "Organiser" & "Participant"

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
}

public class LoginDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class UpdateProfileDto
{
    [MaxLength(80)] public string? FirstName { get; set; }
    [MaxLength(80)] public string? LastName { get; set; }
    [MaxLength(20)] public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    [MaxLength(20)] public string? Gender { get; set; }
    [MaxLength(120)] public string? EmergencyContact { get; set; }
    [MaxLength(500)] public string? EmergencyNotes { get; set; }
}