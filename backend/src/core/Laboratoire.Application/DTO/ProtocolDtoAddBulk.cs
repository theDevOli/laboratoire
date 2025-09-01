using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class ProtocolDtoAddBulk
{
    public int? CashFlowId { get; set; }
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
    [Required]
    public bool? IsCollectedByClient { get; set; }
}
