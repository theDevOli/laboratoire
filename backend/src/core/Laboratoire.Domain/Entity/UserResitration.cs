using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public class UserRegistration
{
    [Required]
    public Guid? UserId { get; set; }
    [Required]
    public string? UserPassword { get; set; }
}
