using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public class Authentication
{
    public Guid? PartnerId { get; set; }
    public Guid? ClientId { get; set; }
    [Required]
    public string? Username { get; set; }
    [Required]
    public string? Name { get; set; }
    [Required]
    public bool? IsActive { get; set; }
    public bool? Protocol { get; set; }
    public bool? Client { get; set; }
    public bool? Property { get; set; }
    public bool? CashFlow { get; set; }
    public bool? Partner { get; set; }
    public bool? Users { get; set; }
    public bool? Chemical { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is null
        || obj.GetType() != this.GetType())
            return false;

        var other = (Authentication)obj;

        return this.Username == other.Username
        && this.Name == other.Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Username);
    }
}
