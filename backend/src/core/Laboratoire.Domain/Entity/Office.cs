using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public class Office
{
    [Required]
    public Guid? OfficeId { get; set; }
    [Required]
    public string? OfficeName { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != typeof(Office))
            return false;

        Office other = (Office)obj;

        return this.OfficeId == other.OfficeId
            && this.OfficeName == other.OfficeName;
    }

    public override int GetHashCode()
    => HashCode.Combine(OfficeId, OfficeName);

}
