using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class PartnerUpdatableService
(
    IPartnerRepository partnerRepository,
    ILogger<PartnerUpdatableService> logger
)
: IPartnerUpdatableService
{
    public async Task<Error> UpdatePartnerAsync(Partner partner)
    {
        logger.LogInformation("Starting update for partner with ID {PartnerId}", partner.PartnerId);

        var exists = await partnerRepository.DoesPartnerExistByIdAsync(partner);
        if (!exists)
        {
            logger.LogWarning("Partner with ID {PartnerId} not found.", partner.PartnerId);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        var ok = await partnerRepository.UpdatePartnerAsync(partner);
        if (!ok)
        {
            logger.LogError("Failed to update partner with ID {PartnerId}.", partner.PartnerId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Partner with ID {PartnerId} updated successfully.", partner.PartnerId);

        return Error.SetSuccess();
    }
}
