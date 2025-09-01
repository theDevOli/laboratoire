using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class CashFlowGetterService
(
    ICashFlowRepository cashFlowRepository,
    ILogger<CashFlowGetterService> logger
)
: ICashFlowGetterService
{
    public  Task<IEnumerable<CashFlow>> GetAllCashFlowAsync()
    {
        logger.LogInformation("Fetching all cash flow records.");
        return  cashFlowRepository.GetAllCashFlowAsync();
    }
}
