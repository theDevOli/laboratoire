using System.ComponentModel.DataAnnotations;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.DTO;

public sealed class ParameterDtoAdd
{
    [Required]
    public int? CatalogId { get; set; }
    [Required]
    public string? ParameterName { get; set; }
    [Required]
    public string? Unit { get; set; }
    [Required]
    public int? InputQuantity { get; set; }
    public string? OfficialDoc { get; set; }
    public string? Vmp { get; set; }
    public string? Equation { get; set; }
}