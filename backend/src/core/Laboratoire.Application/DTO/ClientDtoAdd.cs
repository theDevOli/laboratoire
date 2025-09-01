using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;

public sealed class ClientDtoAdd
{
    [Required]
    public string? ClientName { get; set; }
    public string? ClientTaxId { get; set; }
    [EmailAddress]
    public string? ClientEmail { get; set; }
    [RegularExpression(@"^(\(?\d{2}\)?\s?)?\d{5}-?\d{4}$")]
    public string? ClientPhone { get; set; }
}