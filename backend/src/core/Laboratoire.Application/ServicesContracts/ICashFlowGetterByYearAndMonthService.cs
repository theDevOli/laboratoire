using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface ICashFlowGetterByYearAndMonthService
{
    Task<IEnumerable<CashFlow>> GetCashFlowByYearAndMonthAsync(int? year, int? month);
}
