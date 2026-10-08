using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class Event
    {
        public int EventID { get; set; }

        public int OrganiserID { get; set; }
        public User Organiser { get; set; } = null!;

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

        [Required, MaxLength(20)]
        public string EventType { get; set; } = "Run"; // "Run", "Walk" & "Cycle"

        public int MaxParticipants { get; set; } = 500;

        public DateTime? RegistrationDeadline { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Open"; // "Open", "Closed" & "Completed"

        public string? BannerUrl { get; set; } // Part 3: Azure Blob Storage

        // Navigation
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();

    }
}
