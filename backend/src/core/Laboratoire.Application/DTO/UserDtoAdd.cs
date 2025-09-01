using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class UserDtoAdd
{
    [Required]
    public int? RoleId { get; set; }
    public string? Username { get; set; }

    [Required]
    public bool? IsActive { get; set; } = true;
}