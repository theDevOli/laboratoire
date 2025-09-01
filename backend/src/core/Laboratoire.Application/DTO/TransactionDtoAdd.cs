using System.ComponentModel.DataAnnotations;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.DTO;

public sealed class TransactionDtoAdd
{
    [Required]
    public string? TransactionType { get; set; }
    public string? OwnerName { get; set; }
    public string? BankName { get; set; }
}