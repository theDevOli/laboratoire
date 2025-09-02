using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public sealed class ReportPatch
{
    [Required]
    public Guid? ReportId { get; set; }
    [Required]
    public string? ProtocolId { get; set; }
    [Required]
    public ReportResult[]? Results { get; set; }
}
