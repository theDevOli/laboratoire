using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class ClientGetterService
(
    IClientRepository clientRepository,
    ILogger<ClientGetterService> logger
)
: IClientGetterService
{
    public Task<IEnumerable<Client>> GetAllClientsAsync(string? filter)
    {
        logger.LogInformation("Fetching all clients with filter: {Filter}", filter ?? "(none)");

        return clientRepository.GetAllClientsAsync(filter);
    }
}
