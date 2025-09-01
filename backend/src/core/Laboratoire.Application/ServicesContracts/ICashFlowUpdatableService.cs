using Laboratoire.Domain.Entity;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface ICashFlowUpdatableService
{

    Task<Error> UpdateCashFlowAsync(CashFlow cashFlow);
}
