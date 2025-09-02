using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.PartnerServices;

public class PartnerGetterByIdService
(
    IPartnerRepository partnerRepository,
    ILogger<PartnerGetterByIdService> logger
)
: IPartnerGetterByIdService
{
    public async Task<Partner?> GetPartnerByIdAsync(Guid? partnerId)
    {
        if (partnerId is null)
        {
            logger.LogWarning("GetPartnerByIdAsync called with null partnerId.");
            return null;
        }

        logger.LogInformation("Fetching partner with ID: {PartnerId}", partnerId);
        return await partnerRepository.GetPartnerByIdAsync(partnerId);
    }
}
