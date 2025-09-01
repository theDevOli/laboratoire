using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class AuthDtoToken
{
    [Required]
    public string? Token { get; set; }
}
