using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class ProtocolDtoYears
{
    [Required]
    public int? Year { get; set; }
}
