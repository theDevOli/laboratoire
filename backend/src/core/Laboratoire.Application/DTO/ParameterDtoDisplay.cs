using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class ParameterDtoDisplay
{
    [Required]
    public int? ParameterId { get; set; }
    [Required]
    public string? ParameterName { get; set; }
    [Required]
    public int? InputQuantity { get; set; }
}
