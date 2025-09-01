using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class ProtocolDtoUpdateCashFlow
{
    [Required]
    public string? ProtocolId { get; set; }
    [Required]
    public int? CashFlowId { get; set; }
    [Required]
    public string? Description { get; set; }
}
