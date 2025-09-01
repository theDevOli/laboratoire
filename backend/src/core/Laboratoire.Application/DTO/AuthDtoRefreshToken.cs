using System;
using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class AuthDtoRefreshToken
{
    [Required]
    public string? RefreshToken { get; set; }
}
