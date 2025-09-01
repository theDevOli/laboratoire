using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IClientGetterService
{
    Task<IEnumerable<Client>> GetAllClientsAsync(string? filter);
}
