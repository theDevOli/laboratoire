using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Laboratoire.Domain.Entity;

public class Property
{
    [Required]
    public int? PropertyId { get; set; }
    [Required]
    public Guid? ClientId { get; set; }
    [Required]
    public int? StateId { get; set; }
    [Required]
    public string? PropertyName { get; set; }
    public string? Registration { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Area { get; set; }
    public string? Ccir { get; set; }
    public string? ItrNirf { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != this.GetType())
            return false;

        var other = (Property)obj;

        return other.PropertyId == this.PropertyId
        && other.PropertyName == this.PropertyName
        && other.City == this.City
        && other.Area == this.Area;
    }

    public override int GetHashCode()
    => HashCode.Combine(PropertyId, PropertyName, City, Area);

}
