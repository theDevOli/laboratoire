using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.CropServices;

public class CropGetterByIdService
(
    ICropRepository cropRepository,
    ILogger<CropGetterByIdService> logger
)
: ICropGetterByIdService
{
    public async Task<Crop?> GetCropByIdAsync(int? cropId)
    {
        if (cropId is null)
        {
            logger.LogWarning("GetCropByIdAsync was called with null cropId.");
            return null;
        }

        logger.LogInformation("Fetching crop with ID: {CropId}", cropId);

        return await cropRepository.GetCropByIdAsync(cropId);
    }
}
