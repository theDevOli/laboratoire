using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public class User
{
    [Required]
    public Guid? UserId { get; set; }
    [Required]
    public int? RoleId { get; set; }

    [Required]
    public string? Username { get; set; }

     [Required]
    public bool? IsActive { get; set; } = true;

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != this.GetType())
            return false;

        User other = (User)obj;
        return this.UserId == other.UserId
        && this.Username == other.Username;
    }

    public override int GetHashCode()
    => HashCode.Combine(UserId, Username);
}
