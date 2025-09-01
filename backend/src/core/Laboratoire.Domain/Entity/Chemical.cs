using System.ComponentModel.DataAnnotations;
using Laboratoire.Domain.Utils;

namespace Laboratoire.Domain.Entity;

public class Chemical
{
    [Required]
    public int? ChemicalId { get; set; }

    [Required]
    public string? ChemicalName { get; set; }
    [Required]
    public string? Concentration { get; set; }
    [Required]
    public double? Quantity { get; set; }
    [Required]
    public string? Unit { get; set; }
    [Required]
    public bool? IsPoliceControlled { get; set; }
    [Required]
    public bool? IsArmyControlled { get; set; }
    [Required]
    public DateTime? EntryDate { get; set; }
    [Required]
    [DateGreaterThan("EntryDate")]
    public DateTime? ExpireDate { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != this.GetType())
            return false;

        Chemical? other = obj as Chemical;

        return this.ChemicalName?.ToLower() == other?.ChemicalName?.ToLower()
        && this.Concentration?.ToLower() == other?.Concentration?.ToLower();
    }

    public override int GetHashCode()
    => HashCode.Combine(this.ChemicalName, this.Concentration);
}
