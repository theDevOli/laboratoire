using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class UserDtoRename
{
    [Required]
    public Guid? UserId { get; set; }

    [Required]
    public string? Username { get; set; }

}
