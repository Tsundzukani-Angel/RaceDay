using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs;

public class ResultDto
{
    [Required]
    public int EnrolmentID { get; set; }

    [Required]
    public TimeSpan FinishTime { get; set; }

    [Required, Range(1, 100000)]
    public int Position { get; set; }

    public string Status { get; set; } = "Finished"; // "Finished" | "DNF" | "DNS" | "Disqualified"
}
