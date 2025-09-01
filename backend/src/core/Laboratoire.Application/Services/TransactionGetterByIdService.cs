using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class TransactionGetterByIdService
(
    ITransactionRepository transactionRepository,
    ILogger<TransactionGetterByIdService> logger
)
: ITransactionGetterByIdService
{
    public async Task<Transaction?> GetTransactionByIdAsync(int? transactionId)
    {
        if (transactionId is null)
        {
            logger.LogWarning("GetTransactionByIdAsync called with null transactionId.");
            return null;
        }

        logger.LogInformation("Fetching transaction with ID: {TransactionId}", transactionId);

        return await transactionRepository.GetTransactionByIdAsync(transactionId);
    }
}
