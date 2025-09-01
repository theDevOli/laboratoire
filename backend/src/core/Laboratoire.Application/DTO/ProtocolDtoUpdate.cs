using System.ComponentModel.DataAnnotations;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.DTO;

public sealed class ProtocolDtoUpdate
{
    // private string? _description;
    [Required]
    public string? ProtocolId { get; set; }
    public int? CashFlowId { get; set; }

    [Required]
    public int? TransactionId { get; set; }

    public DateTime? PaymentDate { get; set; }
    public decimal? TotalPaid { get; set; }
    public Guid? ReportId { get; set; }
    [Required]
    public Guid? ClientId { get; set; }
    [Required]
    public int? PropertyId { get; set; }
    public Guid? PartnerId { get; set; }
    [Required]
    public int? CatalogId { get; set; }
    [Required]
    public DateTime? EntryDate { get; set; }
    public DateTime? ReportDate { get; set; }
    public int?[]? Crops { get; set; }
    [Required]
    public bool? IsCollectedByClient { get; set; }
    public ReportResult[]? Results { get; set; }

    public string DisplayMoney()
    => $"R$ {TotalPaid?.ToString("F2")}";

    public bool ToAddCashFlow()
    => CashFlowId is null && TotalPaid is not null;
}