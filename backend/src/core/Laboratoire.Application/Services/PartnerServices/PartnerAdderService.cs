using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Application.Mapper;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.PartnerServices;

public class PartnerAdderService
(
    IPartnerRepository partnerRepository,
    IUserAdderService userAdderService,
    IUserDeletionService userDeletionService,
    ILogger<PartnerAdderService> logger
)
: IPartnerAdderService
{
    public async Task<Error> AddPartnerAsync(PartnerDtoAdd partnerDto)
    {
        logger.LogInformation("Starting to add a new partner with email: {PartnerEmail} and name: {PartnerName}", partnerDto.PartnerEmail, partnerDto.PartnerName);

        var partner = partnerDto.ToPartner();
        var userDto = partnerDto.ToUser();

        var exists = await partnerRepository.DoesPartnerExistByEmailAndNameAsync(partner);
        if (exists)
        {
            logger.LogWarning("Partner with email {PartnerEmail} and name {PartnerName} already exists.", partner.PartnerEmail, partner.PartnerName);
            return Error.SetError(ErrorMessage.ConflictPost, 409);
        }

        var userId = await userAdderService.AddUserAsync(userDto);
        if (userId is null)
        {
            logger.LogError("Failed to add user for partner.");
            return Error.SetError(ErrorMessage.NotFound, 404);
        }
        var ok = await partnerRepository.AddPartnerAsync(partner, userId);

        if (!ok)
        {
            logger.LogError("Failed to add partner.");
            var error = await userDeletionService.DeletionUserAsync(userDto.ToUser(userId));
            if (error.IsNotSuccess())
                logger.LogError("Rollback failed: could not delete user {UserId} after partner insertion failure.", userId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Partner and user successfully added.");
        return Error.SetSuccess();
    }
}
