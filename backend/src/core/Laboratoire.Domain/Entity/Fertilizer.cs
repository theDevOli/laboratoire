using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public class Fertilizer
{
    [Required]
    public int? FertilizerId { get; set; }
    [Required]
    public int? Nitrogen { get; set; }
    [Required]
    public int? Phosphorus { get; set; }
    [Required]
    public int? Potassium { get; set; }
    [Required]
    public bool? IsAvailable { get; set; }
    [Required]
    public string? Proportion { get; set; }
}
