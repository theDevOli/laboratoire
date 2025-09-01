using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface ITransactionGetterByIdService
{
    Task<Transaction?> GetTransactionByIdAsync(int? transactionId);
}
