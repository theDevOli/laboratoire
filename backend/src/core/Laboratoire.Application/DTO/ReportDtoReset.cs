using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class ReportDtoReset
{
    [Required]
    public string? ProtocolId { get; set; }
    [Required]
    public int? CatalogId { get; set; }

}
