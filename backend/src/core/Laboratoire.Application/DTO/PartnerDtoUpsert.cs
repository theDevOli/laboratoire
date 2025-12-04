using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class PartnerDtoUpsert
{
    [Required]
    public Guid? OfficeId { get; set; }
    public string? PartnerName { get; set; }
    [Required]
    public string? PartnerPhone { get; set; }
}