using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class ProtocolDtoPatch
{
    [Required]
    public string? ProtocolId { get; set; }
    [Required]
    public DateTime? EntryDate { get; set; }
    [Required]
    public bool? IsCollectedByClient { get; set; }
    public Guid? PartnerId { get; set; }
    [Required]
    public DateTime? ReportDate { get; set; }

}
