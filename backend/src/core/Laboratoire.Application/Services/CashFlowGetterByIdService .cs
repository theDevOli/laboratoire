using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class CashFlowGetterByIdService
(
    ICashFlowRepository cashFlowRepository,
    ILogger<CashFlowGetterByIdService> logger
)
: ICashFlowGetterByIdService
{

    public async Task<CashFlow?> GetCashFlowByIdAsync(int? cashFlowId)
    {
        logger.LogInformation("Attempting to retrieve CashFlow with ID {CashFlowId}.", cashFlowId);
        if (cashFlowId is null)
            return null;

        return await cashFlowRepository.GetCashFlowByIdAsync(cashFlowId);
    }
}
