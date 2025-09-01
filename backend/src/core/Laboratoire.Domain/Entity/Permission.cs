using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public class Permission
{
    [Required]
    public int? PermissionId { get; set; }
    [Required]
    public int? RoleId { get; set; }
    public bool? Protocol { get; set; }
    public bool? Client { get; set; }
    public bool? Property { get; set; }
    public bool? CashFlow { get; set; }
    public bool? Partner { get; set; }
    public bool? Users { get; set; }
    public bool? Chemical { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is null || this.GetType() != obj.GetType()) return false;

        var other = (Permission)obj;

        return this.PermissionId == other.PermissionId
        && this.RoleId == other.RoleId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.PermissionId, this.RoleId);
    }
}
