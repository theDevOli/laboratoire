using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class TransactionAdderService
(
    ITransactionRepository transactionRepository,
    ILogger<TransactionAdderService> logger
)
: ITransactionAdderService
{
    public async Task<Error> AddTransactionAsync(TransactionDtoAdd transactionDto)
    {
        var transaction = transactionDto.ToTransaction();
        logger.LogInformation("Attempting to add new transaction with type: {TransactionType} and bank: {BankName}",
                transaction.TransactionType, transaction.BankName);
        var exists = await transactionRepository.DoesTransactionExistByUniqueAsync(transaction);
        if (exists)
        {
            logger.LogWarning("Transaction already exists with type: {TransactionType} and bank: {BankName}",
                transaction.TransactionType, transaction.BankName);
            return Error.SetError(ErrorMessage.ConflictPost, 409);
        }

        var ok = await transactionRepository.AddTransactionAsync(transaction);
        if (!ok)
        {
            logger.LogError("Failed to add transaction with type: {TransactionType}", transaction.TransactionType);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Transaction added successfully: {TransactionType}", transaction.TransactionType);
        return Error.SetSuccess();
    }
}
