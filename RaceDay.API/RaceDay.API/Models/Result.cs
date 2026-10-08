using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class Result
    {
        public int ResultID { get; set; }

        public int EnrolmentID { get; set; }
        public Enrolment Enrolment { get; set; } = null!;

        public TimeSpan FinishTime { get; set; }

        [Range(1, 100000)]
        public int Position { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Finished"; // "Finished", "DNF", "DNS" & "Disqualified"

        public DateTime RecordedDate { get; set; } = DateTime.UtcNow;
    }
}
