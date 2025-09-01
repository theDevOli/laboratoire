using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class PropertyDtoAdd
{

    [Required]
    public Guid? ClientId { get; set; }
    [Required]
    public int? StateId { get; set; }
    [Required]
    public string? PropertyName { get; set; }
    public string? Registration { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Area { get; set; }
    public string? Ccir { get; set; }
    public string? ItrNirf { get; set; }
}