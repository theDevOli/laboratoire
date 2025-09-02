using System;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.ClientServices;

public class ClientGetterByLikeTaxIdService
(
    IClientRepository clientRepository,
    ILogger<ClientGetterByLikeTaxIdService> logger
)
: IClientGetterByLikeTaxIdService
{
    public async Task<IEnumerable<Client>> GetClientsByLikeTaxId(string? taxId)
    {
        if (string.IsNullOrWhiteSpace(taxId))
        {
            logger.LogWarning("GetClientsByLikeTaxId called with null or empty taxId.");
            return Array.Empty<Client>();
        }

        logger.LogInformation("Fetching clients with TaxId like: {TaxId}", taxId);

        return await clientRepository.GetClientsLikeAsync(taxId);
    }
}
