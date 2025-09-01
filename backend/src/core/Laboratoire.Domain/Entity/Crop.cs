using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public class Crop
{
    [Required]
    public int? CropId { get; set; }
    [Required]
    public string? CropName { get; set; }
    public int? NitrogenCover { get; set; }
    [Required]
    public int? NitrogenFoundation { get; set; }
    [Required]
    public CropParameter? Phosphorus { get; set; }
    [Required]
    public CropParameter? Potassium { get; set; }
    [Required]
    public int? MinV { get; set; }
    public string? ExtraData { get; set;}

    public override bool Equals(object? obj)
    {
        if (obj is null || this.GetType() != obj.GetType())
            return false;

        Crop other = (Crop)obj;

        return other.CropId == this.CropId
        && other.CropName == this.CropName;
    }

    public override int GetHashCode()
    => HashCode.Combine(this.CropId, this.CropName);
}
