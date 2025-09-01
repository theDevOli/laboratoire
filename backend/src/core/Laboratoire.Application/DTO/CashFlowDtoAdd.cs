using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class CashFlowDtoAdd
{
    [Required]
    public int? TransactionId { get; set; }
    public string? Description { get; set; }
    public Guid? PartnerId { get; set; }

    [Range(-999.99, 999.99)]
    public decimal? TotalPaid { get; set; }
    public DateTime? PaymentDate { get; set; }

}