using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class Enrolment
    {
        public int EnrolmentID { get; set; }

        public int ParticipantID { get; set; }

        public User Participant { get; set; } = null!;

        public int EventID { get; set; }
        public Event Event { get; set; } = null!;

        public int CategoryID { get; set; }

        public Category Category { get; set; } = null!;

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        [MaxLength(20)]
        public string? RaceNumber { get; set; }

        [Required, MaxLength(20)]
        public string PaymentStatus { get; set; } = "Pending"; // "Pending", "Confirmed" & "Cancelled"

        // one-to-one with Result
        public Result? Result { get; set; }
    }
}
