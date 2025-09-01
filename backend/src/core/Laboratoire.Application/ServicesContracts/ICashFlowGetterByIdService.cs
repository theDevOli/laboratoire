using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface ICashFlowGetterByIdService
{

    Task<CashFlow?> GetCashFlowByIdAsync(int? cashFlowId);
}
