using System.ComponentModel.DataAnnotations;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.DTO;

public sealed class CashFlowDtoDisplay
{
    [Required]
    public IEnumerable<CashFlow>? CashFlow { get; set;}
    [Required]
    public decimal? TotalAmount { get; set;}

}
