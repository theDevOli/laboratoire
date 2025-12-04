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
    ILogger<PartnerAdderService> logger
)
: IPartnerAdderService
{
    public async Task<Error> AddPartnerAsync(PartnerDtoUpsert partnerDto)
    {
        logger.LogInformation("Starting to add a new partner with officeId: {OfficeId} and name: {PartnerName}", partnerDto.OfficeId, partnerDto.PartnerName);

        var partner = partnerDto.ToPartner();
        var userDto = partnerDto.ToUser();

        var exists = await partnerRepository.DoesPartnerExistByOfficeAndNameAsync(partner);
        if (exists)
        {
            logger.LogWarning("Partner with name {PartnerName} already exists.", partner.PartnerName);
            return Error.SetError(ErrorMessage.ConflictPost, 409);
        }

        var userId = await userAdderService.AddUserAsync(userDto);
        if (userId is null)
        {
            logger.LogError("Failed to add user for partner.");
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Partner and user successfully added.");
        return Error.SetSuccess();
    }
}
