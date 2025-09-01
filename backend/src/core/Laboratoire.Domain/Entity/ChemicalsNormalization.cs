using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public class ChemicalsNormalization
{
    [Required]
    public int? ChemicalId { get; set; }
    [Required]
    public int? HazardId { get; set; }
}
