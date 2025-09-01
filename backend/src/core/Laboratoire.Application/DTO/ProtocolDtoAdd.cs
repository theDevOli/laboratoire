using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class ProtocolDtoAdd
{
    // private string? _description;
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
    [Required]
    public int? TransactionId { get; set; }
    public decimal? TotalPaid { get; set; }
    public DateTime? PaymentDate { get; set; }

    public string DisplayMoney()
    => $"R$ {TotalPaid?.ToString("F2")}";
}