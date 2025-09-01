using System.ComponentModel.DataAnnotations;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.DTO;

public sealed class CatalogDtoAdd
{
    [Required]
    public string? ReportType { get; set; }
    [Required]
    public string? SampleType { get; set; }
     [Required]
    public string? LabelName { get; set; }
    [Required]
    public Legend[]? Legends { get; set; }
    [Required]
    [Range(0.0, 999.99)]
    public decimal? Price { get; set; }
}
