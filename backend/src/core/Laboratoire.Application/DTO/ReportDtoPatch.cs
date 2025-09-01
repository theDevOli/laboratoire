using System.ComponentModel.DataAnnotations;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.DTO;

public sealed class ReportDtoPatch
{
    [Required]
    public Guid? ReportId { get; set; }
    [Required]
    public ReportResult[]? Results { get; set; }
}