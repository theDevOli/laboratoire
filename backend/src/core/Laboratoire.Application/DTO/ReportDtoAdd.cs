using System.ComponentModel.DataAnnotations;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.DTO;

public sealed class ReportDtoAdd
{

    [Required]
    public string? ProtocolId { get; set; }
    [Required]
    public ReportResult[]? Results { get; set; }
}