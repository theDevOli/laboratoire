using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class CropDtoDb
{
    [Required]
    public int? CropId { get; set; }
    [Required]
    public string? CropName { get; set; }
    [Required]
    public int? NitrogenCover { get; set; }
    [Required]
    public int? NitrogenFoundation { get; set; }
    [Required]
    public string? Phosphorus { get; set; }
    [Required]
    public string? Potassium { get; set; }
    [Required]
    public int? MinV { get; set; }
    public string? ExtraData { get; set; }
}
