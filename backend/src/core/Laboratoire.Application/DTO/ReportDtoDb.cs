using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public  sealed class ReportDtoDb
{
    [Required]
    public Guid? ReportId { get; set; }
    [Required]
    public string[]? Results { get; set; }
}