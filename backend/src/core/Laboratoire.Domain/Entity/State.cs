using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public class State
{
    [Required]
    public int? StateId { get; set; }
    [Required]
    public string? StateName { get; set; }
    [Required]
    public string? StateCode { get; set; }
}
