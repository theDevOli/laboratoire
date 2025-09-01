using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public class CropsNormalization
{
    [Required]
    public int? CropId { get; set; }
    [Required]
    public string? ProtocolId { get; set; }

}
