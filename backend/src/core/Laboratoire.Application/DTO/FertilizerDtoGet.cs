using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class FertilizerDtoGet
{
    [Required]
    public int? FertilizerId { get; set; }
    [Required]
    public string? Proportion { get; set; }
    [Required]
    public string? Formulation { get; set; }
}
