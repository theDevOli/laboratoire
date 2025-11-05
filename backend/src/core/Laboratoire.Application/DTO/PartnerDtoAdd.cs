using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class PartnerDtoAdd
{
    [Required]
    public Guid? OfficeId { get; set; }
    public string? PartnerName { get; set; }
    [Required]
    public string? PartnerPhone { get; set; }
    [Required]
    public string? Username { get; set; }
    [Required]
    public bool? IsActive { get; set; }
}