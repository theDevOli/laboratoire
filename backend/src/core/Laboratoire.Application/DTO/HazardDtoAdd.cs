using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class HazardDtoAdd
{

    [Required]
    public string? HazardClass { get; set; }
    [Required]
    public string? HazardName { get; set; }
}