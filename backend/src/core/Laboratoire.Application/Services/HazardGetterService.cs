using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class HazardGetterService
(
    IHazardRepository hazardRepository,
    ILogger<HazardGetterService> logger
)
: IHazardGetterService
{
    public Task<IEnumerable<Hazard>> GetAllHazardsAsync()
    {
        logger.LogInformation("Fetching all hazards from the repository.");
        return hazardRepository.GetAllHazardsAsync();
    }
}
