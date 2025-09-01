using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public class CashFlow
{
    private string? _description;
    [Required]
    public int? CashFlowId { get; set; }
    [Required]
    public int? TransactionId { get; set; }
    public string? Description
    {
        get { return _description; }
        set
        {
            if (string.IsNullOrEmpty(value))
                _description = $"{(TotalPaid > 0 ? "Entrada" : "Saída")} de: {TotalPaid:C2)}";
            else
                _description = value;
        }

    }
    public Guid? PartnerId { get; set; }

    [Range(-999.99, 999.99)]
    public decimal? TotalPaid { get; set; }
    public DateTime? PaymentDate { get; set; }


    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != this.GetType())
            return false;
        CashFlow other = (CashFlow)obj;
        return TransactionId == other.TransactionId &&
        TotalPaid == other.TotalPaid &&
        PaymentDate == other.PaymentDate;
    }

    public override int GetHashCode()
    => HashCode.Combine(CashFlowId, PaymentDate);
}
