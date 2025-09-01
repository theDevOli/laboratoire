using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class UserDtoChangePassword
{
    [Required]
    public Guid? UserId { get; set; }
    [Required]
    public string? UserPassword { get; set; }
    [Required]
    [Compare("UserPassword")]
    public string? ConfirmPassword { get; set; }
    [Required]
    public string? OldPassword { get; set; }
}
