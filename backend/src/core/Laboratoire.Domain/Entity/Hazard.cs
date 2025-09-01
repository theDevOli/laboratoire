
using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public class Hazard
{
    [Required]
    public int? HazardId { get; set; }
    [Required]
    public string? HazardClass { get; set; }
    [Required]
    public string? HazardName { get; set; }

    public override string ToString()
    => $"{this.HazardClass} - {this.HazardName}";

    public override bool Equals(object? obj)
    {
        if (obj is null || this.GetType() != obj.GetType())
            return false;

        var other = obj as Hazard;

        return this.HazardName == other?.HazardName && this.HazardClass == other?.HazardClass;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.HazardClass, this.HazardName);
    }
}
