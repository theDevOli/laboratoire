using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface ICashFlowAdderService
{
Task<Error> AddCashFlowAsync(CashFlow cashFlow,Protocol? protocol);
}
