using System.ComponentModel.DataAnnotations;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.Utils;

namespace Laboratoire.Application.DTO;

public sealed class ChemicalDtoAdd
{
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
    public int?[]? Hazards { get; set; }
}