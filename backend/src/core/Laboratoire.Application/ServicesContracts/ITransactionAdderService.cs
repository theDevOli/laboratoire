using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface ITransactionAdderService
{
    Task<Error> AddTransactionAsync(TransactionDtoAdd transaction);
}
