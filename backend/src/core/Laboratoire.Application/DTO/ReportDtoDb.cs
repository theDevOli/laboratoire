using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.DTO;

public  sealed class ReportDtoDb
{
    [Required]
    public Guid? ReportId { get; set; }
    [Required]
    public string? ProtocolId { get; set; }
    [Required]
    public string[]? Results { get; set; }
}