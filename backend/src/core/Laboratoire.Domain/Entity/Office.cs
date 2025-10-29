using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public class Office
{
    [Required]
    public Guid? OfficeId { get; set; }
    [Required]
    public string? OfficeName { get; set; }
    public string? OfficeEmail { get; set; }
    public string? City { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != typeof(Office))
            return false;

        Office other = (Office)obj;

        return this.OfficeName == other.OfficeName
            && this.City == other.City;
    }

    public override int GetHashCode()
    => HashCode.Combine(OfficeId, OfficeName);

}
