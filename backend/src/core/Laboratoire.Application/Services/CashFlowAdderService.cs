using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class CashFlowAdderService
(
    ICashFlowRepository cashFlowRepository,
    IProtocolRepository protocolRepository,
    ILogger<CashFlowAdderService> logger
)
: ICashFlowAdderService
{

    public async Task<Error> AddCashFlowAsync(CashFlow cashFlow, Protocol? protocol)
    {
        logger.LogInformation("Starting to add cash flow.");
        var cashFlowId = await cashFlowRepository.AddCashFlowAsync(cashFlow);
        logger.LogInformation("Cash flow added with ID {CashFlowId}.", cashFlowId);

        if (protocol is not null)
        {
            protocol.CashFlowId = cashFlowId;
            var isUpdated = await protocolRepository.UpdateCashFlowIdAsync(protocol);
            if (!isUpdated)
            {
                logger.LogError("Failed to update protocol with CashFlowId {CashFlowId}.", cashFlowId);
                return Error.SetError(ErrorMessage.DbError, 500);
            }
        }

        logger.LogInformation("Cash flow addition process completed successfully.");
        return Error.SetSuccess();
    }
}
