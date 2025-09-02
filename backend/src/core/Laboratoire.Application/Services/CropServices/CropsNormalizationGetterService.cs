using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.CropServices;

public class CropsNormalizationGetterService
(
    ICropsNormalizationRepository cropsNormalizationRepository,
    ILogger<CropsNormalizationGetterService> logger
)
: ICropsNormalizationGetterService
{
    public Task<IEnumerable<CropsNormalization>> GetAllCropsAsync()
    {
        logger.LogInformation("Fetching all crop normalizations from repository.");
        return cropsNormalizationRepository.GetAllCropsAsync();
    }
}
