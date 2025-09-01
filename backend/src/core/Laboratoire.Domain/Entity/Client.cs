using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public class Client
{
    [Required]
    public Guid? ClientId { get; set; }
    [Required]
    public string? ClientName { get; set; }
    [Required]
    public string? ClientTaxId { get; set; }
    [EmailAddress]
    public string? ClientEmail { get; set; }
    [RegularExpression(@"^(\(?\d{2}\)?\s?)?\d{5}-?\d{4}$")]
    public string? ClientPhone { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != this.GetType()) return false;
        var other = obj as Client;
        return this.ClientTaxId == other?.ClientTaxId &&
        this.ClientEmail == other?.ClientEmail;
    }

    public override int GetHashCode()
    => HashCode.Combine(this.ClientTaxId, this.ClientId);

}
