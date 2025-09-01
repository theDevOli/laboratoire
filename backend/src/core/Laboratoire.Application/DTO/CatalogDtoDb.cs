using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class CatalogDtoDb
{
    [Required]
    public int? CatalogId { get; set; }
    [Required]
    public string? ReportType { get; set; }
    [Required]
    public string? SampleType { get; set; }
     [Required]
    public string? LabelName { get; set; }
    [Required]
    public string[]? Legends { get; set; }
    [Required]
    [Range(0.0, 999.99)]
    public decimal? Price { get; set; }
}
