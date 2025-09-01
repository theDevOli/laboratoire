using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class PartnerDtoAdd
{
    public string? PartnerName { get; set; }
    [Required]
    public string? OfficeName { get; set; }
    [Required]
    public string? PartnerPhone { get; set; }
    [EmailAddress]
    public string? PartnerEmail { get; set; }
    [Required]
    public string? Username { get; set; }
    [Required]
    public bool? IsActive { get; set; }
}