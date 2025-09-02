using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.ProtocolServices;

public class ProtocolSpotSaverService
(
    IProtocolRepository protocolRepository,
    IClientGetterByTaxIdService clientGetterByTaxIdService,
    ILogger<ProtocolSpotSaverService> logger
) : IProtocolSpotSaverService
{
    public async Task<Error> SaveProtocolSpotAsync(int? quantity)
    {
        logger.LogInformation("Starting protocol spot reservation with quantity: {Quantity}", quantity);

        var client = await clientGetterByTaxIdService.GetClientByTaxIdAsync("00000000000");
        if (client is null)
        {
            logger.LogWarning("Client with tax ID '00000000000' not found.");
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        Guid? clientId = client?.ClientId;
        int propertyId = 1;
        logger.LogInformation("Attempting to save protocol spot for client ID: {ClientId} and property ID: {PropertyId}", clientId, propertyId);

        var ok = await protocolRepository.SaveProtocolSpotAsync(quantity, clientId, propertyId);
        if (!ok)
        {
            logger.LogError("Failed to save protocol spot for client ID: {ClientId}", clientId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Protocol spot saved successfully for client ID: {ClientId}", clientId);
        return Error.SetSuccess();
    }
}
