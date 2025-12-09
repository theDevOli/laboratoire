using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public class OfficeDtoUpsert
{
    [Required]
    public string? OfficeName { get; set; }
    public string? OfficeEmail { get; set; }
    public string? City { get; set; }
}
