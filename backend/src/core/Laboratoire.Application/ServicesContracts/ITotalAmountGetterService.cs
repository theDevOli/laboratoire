namespace Laboratoire.Application.ServicesContracts;

public interface ITotalAmountGetterService
{
    Task<decimal?> GetAmountAsync(int? year, int? month, string? cashFlow, int? transaction);
}
