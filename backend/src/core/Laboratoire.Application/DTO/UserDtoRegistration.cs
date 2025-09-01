using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class UserDtoRegistration
{
    [Required]
    public string? Username { get; set; }
    [Required]
    public string? UserPassword { get; set; }
    [Required]
    [Compare("UserPassword")]
    public string? ConfirmPassword { get; set; }
}
