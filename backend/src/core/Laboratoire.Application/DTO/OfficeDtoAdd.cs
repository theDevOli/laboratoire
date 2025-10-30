using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public class OfficeDtoAdd
{
    [Required]
    public string? OfficeName { get; set; }
    public string? OfficeEmail { get; set; }
    public string? City { get; set; }
}
