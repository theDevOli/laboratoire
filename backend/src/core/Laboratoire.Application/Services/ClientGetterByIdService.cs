using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class ClientGetterByIdService
(
    IClientRepository clientRepository,
    ILogger<ClientGetterByIdService> logger
)
: IClientGetterByIdService
{
    public async Task<Client?> GetClientByIdAsync(Guid? clientId)
    {
        if (clientId is null)
        {
            logger.LogWarning("GetClientByIdAsync was called with null clientId.");
            return null;
        }

        logger.LogInformation("Fetching client with ID: {ClientId}", clientId);

        return await clientRepository.GetByClientIdAsync(clientId);
    }
}
