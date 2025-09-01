using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public class Partner
{
    [Required]
    public Guid? PartnerId { get; set; }
    [Required]
    public string? PartnerName { get; set; }
    [Required]
    public string? OfficeName { get; set; }
    [Required]
    public string? PartnerPhone { get; set; }
    [EmailAddress]
    public string? PartnerEmail { get; set; }
    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != this.GetType())
            return false;

        Partner other = (Partner)obj;

        return other.PartnerId == this.PartnerId
        && other.PartnerName == this.PartnerName
        && other.PartnerEmail == this.PartnerEmail;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PartnerName, PartnerId, PartnerEmail);
    }

}
