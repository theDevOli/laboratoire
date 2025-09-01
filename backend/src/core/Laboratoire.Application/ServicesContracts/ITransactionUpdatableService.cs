using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface ITransactionUpdatableService
{
    Task<Error> UpdateTransactionAsync(Transaction transaction);
}
