using System.ComponentModel.DataAnnotations;
using Laboratoire.Domain.Utils;

namespace Laboratoire.Domain.Entity;

public class Protocol
{
    [Required]
    public string? ProtocolId { get; set; }
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
    [DateGreaterThan("EntryDate")]
    public DateTime? ReportDate { get; set; }
    [Required]
    public bool? IsCollectedByClient { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != this.GetType())
            return false;
        var other = (Protocol)obj;

        return other.ClientId == this.ClientId
        && other.PropertyId == this.PropertyId
        && other.CashFlowId == this.CashFlowId
        && other.EntryDate == this.EntryDate;
    }
    public bool IsNotSameCatalog(object? obj)
    {
        if (obj is null || obj.GetType() != this.GetType())
            return false;
        var other = (Protocol)obj;

        return other.CatalogId != this.CatalogId;
    }

    public bool ToAddCashFlow()
    => CashFlowId is null;

    public override int GetHashCode()
    => HashCode.Combine(this.ClientId, this.PropertyId);
}
