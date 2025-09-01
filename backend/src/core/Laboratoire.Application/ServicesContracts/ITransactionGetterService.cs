using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface ITransactionGetterService
{
    Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
}
