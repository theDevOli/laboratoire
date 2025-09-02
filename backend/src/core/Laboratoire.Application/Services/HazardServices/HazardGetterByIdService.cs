using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.HazardServices;

public class HazardGetterByIdService
(
    IHazardRepository hazardRepository,
    ILogger<HazardGetterByIdService> logger
)
: IHazardGetterByIdService
{
    public async Task<Hazard?> GetHazardByIdAsync(int? hazardId)
    {
        if (hazardId is null)
        {
            logger.LogWarning("GetHazardByIdAsync was called with null hazardId.");
            return null;
        }

        logger.LogInformation("Fetching hazard with ID: {HazardId}", hazardId);

        return await hazardRepository.GetHazardByIdAsync(hazardId);
    }
}
