using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class Category
    {
        public int CategoryID { get; set; }

        public int EventID { get; set; }
        public Event Event { get; set; } = null!;

        [Required, MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty; // e.g. "Senior", "10km"

        [MaxLength(50)]
        public string? CategoryType { get; set; } // "Age" | "Distance"

        [Range(0, 10000)]
        public decimal EntryFee { get; set; }

        // Navigation
        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();

    }
}
