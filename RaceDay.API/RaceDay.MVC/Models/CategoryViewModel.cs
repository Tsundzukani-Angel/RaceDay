using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models;

public class CategoryViewModel
{
    public int EventID { get; set; }

    [Required, MaxLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? CategoryType { get; set; } // "Age" or "Distance"

    [Range(0, 10000)]
    public decimal EntryFee { get; set; }
}