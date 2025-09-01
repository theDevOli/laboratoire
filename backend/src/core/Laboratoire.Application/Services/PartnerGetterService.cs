using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class PartnerGetterService
(
    IPartnerRepository partnerRepository,
    ILogger<PartnerGetterService> logger
)
: IPartnerGetterService
{
    public Task<IEnumerable<Partner>> GetAllPartnersAsync()
    {
        logger.LogInformation("Fetching all partners from repository.");
        return partnerRepository.GetAllPartnersAsync();
    }
}
