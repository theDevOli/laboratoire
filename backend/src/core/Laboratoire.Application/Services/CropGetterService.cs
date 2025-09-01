using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class CropGetterService
(
    ICropRepository cropRepository,
    ILogger<CropGetterService> logger
)
: ICropGetterService
{
    public Task<IEnumerable<Crop>> GetAllCropsAsync()
    {
        logger.LogInformation("Fetching all crops from repository.");

        return cropRepository.GetAllCropsAsync();
    }
}
