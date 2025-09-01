using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class CashFlowUpdatableService
(
    ICashFlowRepository cashFlowRepository,
    ILogger<CashFlowUpdatableService> logger
)
: ICashFlowUpdatableService
{
    public async Task<Error> UpdateCashFlowAsync(CashFlow cashFlow)
    {
        logger.LogInformation("Attempting to update CashFlow with ID: {CashFlowId}", cashFlow.CashFlowId);
        var exists = await cashFlowRepository.DoesCashFlowExistsAsync(cashFlow);
        if (!exists)
        {
            logger.LogWarning("CashFlow with ID: {CashFlowId} was not found.", cashFlow.CashFlowId);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        var ok = await cashFlowRepository.UpdateCashFlowAsync(cashFlow);
        if (!ok)
        {
            logger.LogError("Failed to update CashFlow with ID: {CashFlowId}",cashFlow.CashFlowId);;
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Successfully updated CashFlow with ID: {CashFlowId}", cashFlow.CashFlowId);
        return Error.SetSuccess();
    }
}
