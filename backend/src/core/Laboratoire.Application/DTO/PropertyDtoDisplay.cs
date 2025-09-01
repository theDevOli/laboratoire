using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class PropertyDtoDisplay
{
    [Required]
    public int? PropertyId { get; set; }
    [Required]
    public Guid? ClientId { get; set; }
    [Required]
    public string? ClientName { get; set; }
    [Required]
    public string? ClientTaxId { get; set; }
    public int? StateId { get; set; }
    [Required]
    public string? StateCode { get; set; }
    public string? PropertyName { get; set; }
    [Required]
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Registration { get; set; }
    public string? Area { get; set; }
    public string? Ccir { get; set; }
    public string? ItrNirf { get; set; }

}
