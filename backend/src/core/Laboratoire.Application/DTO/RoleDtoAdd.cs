using System.ComponentModel.DataAnnotations;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.DTO;

public sealed class RoleDtoAdd
{
    [Required]
    public string? RoleName { get; set; }
}