using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.ClientServices;

public class ClientGetterByTaxIdService
(
    IClientRepository clientRepository,
    ILogger<ClientGetterByTaxIdService> logger
)
: IClientGetterByTaxIdService
{
    public async Task<Client?> GetClientByTaxIdAsync(string? clientTaxId)
    {
        if (clientTaxId is null)
        {
            logger.LogWarning("GetClientByTaxIdAsync called with null clientTaxId.");
            return null;
        }

        logger.LogInformation("Fetching client with TaxId: {ClientTaxId}", clientTaxId);

        return await clientRepository.GetByTaxIdAsync(clientTaxId);
    }
}
