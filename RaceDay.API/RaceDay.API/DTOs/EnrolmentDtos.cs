using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs;

public class EnrolmentDto
{
    [Required]
    public int EventID { get; set; }

    [Required]
    public int CategoryID { get; set; }
}

public class UpdateEnrolmentStatusDto
{
    [Required]
    public string PaymentStatus { get; set; } = "Confirmed"; // "Pending", "Confirmed" & "Cancelled"

    [MaxLength(20)]
    public string? RaceNumber { get; set; }
}