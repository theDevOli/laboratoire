using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class CashFlowDtoUpsert
{
    [Required]
    public int? TransactionId { get; set; }
    public string? Description { get; set; }
    public Guid? PartnerId { get; set; }
    public decimal? TotalPaid { get; set; }
    public DateTime? PaymentDate { get; set; }

}