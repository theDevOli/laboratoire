using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Application.Mapper;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class PartnerAdderService
(
    IPartnerRepository partnerRepository,
    IUserAdderService userAdderService,
    ILogger<PartnerAdderService> logger
)
: IPartnerAdderService
{
    public async Task<Error> AddPartnerAsync(PartnerDtoAdd partnerDto)
    {
        logger.LogInformation("Starting to add a new partner with email: {PartnerEmail} and name: {PartnerName}", partnerDto.PartnerEmail, partnerDto.PartnerName);

        var partner = partnerDto.ToPartner();
        var user = partnerDto.ToUser();

        var exists = await partnerRepository.DoesPartnerExistByEmailAndNameAsync(partner);
        if (exists)
        {
            logger.LogWarning("Partner with email {PartnerEmail} and name {PartnerName} already exists.", partner.PartnerEmail, partner.PartnerName);
            return Error.SetError(ErrorMessage.ConflictPost, 409);
        }

        var userId = await userAdderService.AddUserAsync(user);
        if (userId is null)
        {
            logger.LogError("Failed to add user for partner.");
            return Error.SetError(ErrorMessage.NotFound, 404);
        }
        var ok = await partnerRepository.AddPartnerAsync(partner, userId);

        if (!ok)
        {
            logger.LogError("Failed to add partner.");
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Partner and user successfully added.");
        return Error.SetSuccess();
    }
}
