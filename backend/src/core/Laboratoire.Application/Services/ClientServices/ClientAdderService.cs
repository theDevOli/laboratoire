using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;



namespace Laboratoire.Application.Services.ClientServices;

public class ClientAdderService
(
    IClientRepository clientRepository,
    IUserAdderService userAdderService,
    IUserDeletionService userDeletionService,
    ILogger<ClientAdderService> logger
)
: IClientAdderService
{
    public async Task<Error> AddClientAsync(ClientDtoAdd clientDto)
    {
        logger.LogInformation("Starting client addition process with ClientTaxId: {ClientTaxId}", clientDto.ClientTaxId);
        var client = clientDto.ToClient();
        var userDto = clientDto.ToUser();

        var exists = await clientRepository.DoesClientExistByTaxIdAsync(client);
        if (exists)
        {
            logger.LogWarning("Client with ClientTaxId {ClientTaxId} already exists.", clientDto.ClientTaxId);
            return Error.SetError(ErrorMessage.ConflictPost, 409);
        }


        logger.LogInformation("Client with ClientTaxId {ClientTaxId} added successfully. Starting addition of associated user.", clientDto.ClientTaxId);
        // TODO:Atomicity
        var userId = await userAdderService.AddUserAsync(userDto);
        if (userId is null)
        {
            logger.LogError("Failed to add user associated with client ClientTaxId {ClientTaxId}.", clientDto.ClientTaxId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        var ok = await clientRepository.AddClientAsync(client, userId);
        if (!ok)
        {
            logger.LogError("Failed to add client with ClientTaxId {ClientTaxId} to the database.", clientDto.ClientTaxId);
            await userDeletionService.DeletionUserAsync(userDto.ToUser(userId));
            return Error.SetError(ErrorMessage.DbError, 500);
        }
        return Error.SetSuccess();
    }
}
