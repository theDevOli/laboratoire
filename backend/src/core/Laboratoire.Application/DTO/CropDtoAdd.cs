using System.ComponentModel.DataAnnotations;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.DTO;

public sealed class CropDtoAdd
{
    [Required]
    public string? CropName { get; set; }
    [Required]
    public int? NitrogenCover { get; set; }
    [Required]
    public int? NitrogenFoundation { get; set; }
    [Required]
    public CropParameter? Phosphorus { get; set; }
    [Required]
    public CropParameter? Potassium { get; set; }
    [Required]
    public int? MinV { get; set; }
    public string? ExtraData { get; set; }
}
