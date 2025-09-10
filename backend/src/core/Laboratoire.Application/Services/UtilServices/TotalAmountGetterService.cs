
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.UtilServices;

public class TotalAmountGetterService
(
    ICashFlowRepository cashFlowRepository,
    ILogger<TotalAmountGetterService> logger
)
: ITotalAmountGetterService
{
    public Task<decimal?> GetAmountAsync(int? year, int? month, string? cashFlow, int? transaction)
    {
        logger.LogInformation("Fetching total amount with filters - Year: {Year}, Month: {Month}, CashFlow: {CashFlow}, Transaction: {Transaction}",
            year, month, cashFlow, transaction);

        return cashFlowRepository.GetAmountAsync(year, month, cashFlow, transaction);
    }
}
