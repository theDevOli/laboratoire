using Laboratoire.Application.Mapper;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class ClientUpdatableService
(
    IClientRepository clientRepository,
    IUserGetterByUsernameService userGetterByUsernameService,
    IUserRenameService userRenameService,
    ILogger<ClientUpdatableService> logger
)
: IClientUpdatableService
{
    public async Task<Error> UpdateClientAsync(Client client)
    {
        logger.LogInformation("Starting update process for client ID: {ClientId}", client.ClientId);
        Client? clientDb = await clientRepository.GetByClientIdAsync(client.ClientId);

        if (clientDb is null)
        {
            logger.LogWarning("Client with ID {ClientId} not found.", client.ClientId);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        if (client.ClientTaxId != clientDb.ClientTaxId)
        {
            logger.LogInformation("Client TaxId change detected: {Old} → {New}", clientDb.ClientTaxId, client.ClientTaxId);
            // var exists = await clientRepository.DoesClientExistByTaxIdAsync(client);
            // if (exists)
            // {
            //     logger.LogWarning("Client with TaxId {TaxId} already exists. Conflict on update.", client.ClientTaxId);
            //     return Error.SetError(ErrorMessage.ConflictPut, 409);
            // }

            var user = await userGetterByUsernameService.GetUserByUsernameAsync(clientDb.ClientTaxId);

            var dto = user?.ToUserRename()!;
            await userRenameService.UserRenameAsync(dto);
        }

        var ok = await clientRepository.UpdateClientAsync(client);
        if (!ok)
        {
            logger.LogError("Failed to update client with ID {ClientId}", client.ClientId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Client with ID {ClientId} updated successfully.", client.ClientId);

        return Error.SetSuccess();
    }
}
