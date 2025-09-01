
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class TransactionGetterService
(
    ITransactionRepository transactionRepository,
    ILogger<TransactionGetterService> logger
)
: ITransactionGetterService
{
    public Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
    {
        logger.LogInformation("Fetching all transactions.");

        return transactionRepository.GetAllTransactionsAsync();
    }
}
