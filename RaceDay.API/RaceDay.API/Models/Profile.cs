using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class Profile
    {
        public int ProfileID { get; set; }

        public int UserID { get; set; }
        public User User { get; set; } = null!;

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(20)]
        public string? Gender { get; set; }

        [MaxLength(120)]
        public string? EmergencyContact { get; set; }

        [MaxLength(500)]
        public string? EmergencyNotes { get; set; }
    }
}
