using Laboratoire.Domain.Entity;

namespace Laboratoire.Domain.RepositoryContracts;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
    Task<Transaction?> GetTransactionByIdAsync(int? transactionId);
    Task<Transaction?> GetUniqueTransactionAsync(Transaction transaction);
    Task<bool> DoesTransactionExistByIdAsync(Transaction transaction);
    Task<bool> DoesTransactionExistByUniqueAsync(Transaction transaction);
    Task<bool> AddTransactionAsync(Transaction transaction);
    Task<bool> UpdateTransactionAsync(Transaction transaction);
}
