using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class TransactionUpdatableService
(
    ITransactionRepository transactionRepository,
    ILogger<TransactionUpdatableService> logger
)
: ITransactionUpdatableService
{
    public async Task<Error> UpdateTransactionAsync(Transaction transaction)
    {
        logger.LogInformation("Starting update for transaction ID: {TransactionId}", transaction.TransactionId);

        var exists = await transactionRepository.DoesTransactionExistByIdAsync(transaction);
        if (!exists)
        {
            logger.LogWarning("Transaction with ID {TransactionId} not found.", transaction.TransactionId);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        var isConflict = await transactionRepository.DoesTransactionExistByUniqueAsync(transaction);
        if (isConflict)
        {
            logger.LogWarning("Transaction conflict detected for type: {TransactionType} and bank: {BankName}",
                transaction.TransactionType, transaction.BankName);
            return Error.SetError(ErrorMessage.ConflictPut, 409);
        }

        var ok = await transactionRepository.UpdateTransactionAsync(transaction);
        if (!ok)
        {
            logger.LogError("Failed to update transaction with ID {TransactionId}.", transaction.TransactionId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Transaction with ID {TransactionId} updated successfully.", transaction.TransactionId);
        return Error.SetSuccess();
    }
}
