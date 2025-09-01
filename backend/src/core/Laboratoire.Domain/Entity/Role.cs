using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public class Role
{
    [Required]
    public int? RoleId { get; set; }
    [Required]
    public string? RoleName { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != this.GetType()) return false;
        Role other = (Role)obj;
        return this.RoleName == other.RoleName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.RoleId, this.RoleName);
    }
}
