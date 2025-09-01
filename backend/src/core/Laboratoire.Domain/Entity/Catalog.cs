using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public class Catalog
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
    public Legend[]? Legends { get; set; }
    [Required]
    [Range(0.0, 999.99)]
    public decimal? Price { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != this.GetType())
            return false;

        Catalog other = (Catalog)obj;

        return this.CatalogId == other.CatalogId
        && this.ReportType == other.ReportType
        && this.SampleType == other.SampleType;
    }

    public override int GetHashCode()
    => HashCode.Combine(CatalogId, ReportType, SampleType);
}
