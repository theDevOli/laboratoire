using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface ICashFlowGetterService
{

    Task<IEnumerable<CashFlow>> GetAllCashFlowAsync();
}
