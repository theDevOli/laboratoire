using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public class Transaction
{
    [Required]
    public int? TransactionId { get; set; }
    [Required]
    public string? TransactionType { get; set; }
    public string? OwnerName { get; set; }
    public string? BankName { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != this.GetType())
            return false;

        var other = (Transaction)obj;

        return other.TransactionId == this.TransactionId;
    }

    public override int GetHashCode()
    => HashCode.Combine(this.TransactionId);
}
