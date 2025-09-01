using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class AuthDtoResetPassword
{
    [Required]
    public Guid? UserId { get; set; }
}
