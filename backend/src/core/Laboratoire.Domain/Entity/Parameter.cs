
using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public class Parameter
{
    [Required]
    public int? ParameterId { get; set; }
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

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != this.GetType())
            return false;

        Parameter other = (Parameter)obj;

        return this.ParameterId == other.ParameterId
        && this.CatalogId == other.CatalogId;
    }

    public override int GetHashCode()
    => HashCode.Combine(ParameterId, CatalogId);
}
