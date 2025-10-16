using System.ComponentModel.DataAnnotations;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.DTO;

public sealed class UserDtoAdd
{
    [Required]
    public int? RoleId { get; set; }
    public string? Username { get; set; }

    [Required]
    public bool? IsActive { get; set; } = true;
    // FIXME: Maybe an error
    public Client? Client{ get; set; }
}