using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public class UserLogin
{
    [Required]
    public string? Username { get; set; }
    [Required]
    public string? UserPassword { get; set; }
}
