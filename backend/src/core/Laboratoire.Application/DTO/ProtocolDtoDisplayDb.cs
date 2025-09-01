using System.ComponentModel.DataAnnotations;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.DTO;

public sealed class ProtocolDtoDisplayDb
{
    [Required]
    public string? ProtocolId { get; set; }
    public Guid? ReportId { get; set; }
    public int? CashFlowId { get; set; }
    [Required]
    public DateTime? EntryDate { get; set; }
    [Required]
    public DateTime? ReportDate { get; set; }
    [Required]
    public bool? IsCollectedByClient { get; set; }
    [Required]
    public int? TransactionId { get; set; }
    public decimal? TotalPaid { get; set; }
    [Required]
    public DateTime? PaymentDate { get; set; }
    [Required]
    public Guid? ClientId { get; set; }
    [Required]
    public string? ClientName { get; set; }
    [Required]
    public string? ClientTaxId { get; set; }
    [Required]
    public int? PropertyId { get; set; }
    public int? StateId { get; set; }
    [Required]
    public string? StateCode { get; set; }

    [Required]
    public string? City { get; set; }
    [Required]
    public string? PropertyName { get; set; }
    public string? PostalCode { get; set; }
    public string? Area { get; set; }
    public string? Ccir { get; set; }
    public string? ItrNirf { get; set; }
    public string[]? Results { get; set; }
    public Guid? PartnerId { get; set; }
    public string? PartnerName { get; set; }
    [Required]
    public int? CatalogId { get; set; }
    [Required]
    public decimal? Price { get; set; }
    [Required]
    public string? ReportType { get; set; }
    public int?[]? Crops { get; set; }
}
