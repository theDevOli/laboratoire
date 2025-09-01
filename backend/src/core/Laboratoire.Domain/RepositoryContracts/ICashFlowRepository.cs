using Laboratoire.Domain.Entity;

namespace Laboratoire.Domain.RepositoryContracts;

public interface ICashFlowRepository
{
    Task<IEnumerable<CashFlow>> GetAllCashFlowAsync();
    Task<IEnumerable<CashFlow>> GetCashFlowByYearAndMonthAsync(int? year, int? month);
    Task<CashFlow?> GetCashFlowByIdAsync(int? cashFlowId);
    Task<int> AddCashFlowAsync(CashFlow cashFlow);
    Task<bool> DoesCashFlowExistsAsync(CashFlow cashFlow);
    Task<bool> UpdateCashFlowAsync(CashFlow cashFlow);
    Task<bool> PatchDescriptionAsync(CashFlow cashFlow);
    Task<decimal?> GetAmountAsync(int? year, int? month, string? cashFlow, int? transaction);
}
