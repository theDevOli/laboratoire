using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.CropServices;

public class CropUpdatableService
(
    ICropRepository cropRepository,
    ILogger<CropUpdatableService> logger
)
: ICropUpdatableService
{
    public async Task<Error> UpdateCropAsync(Crop crop)
    {
        logger.LogInformation("Starting update process for crop ID: {CropId}, Name: {CropName}", crop.CropId, crop.CropName);
        var isConflict = await cropRepository.DoesCropExistByNameAsync(crop);
        if (isConflict)
        {
            logger.LogWarning("Conflict: Another crop with name '{CropName}' already exists.", crop.CropName);
            return Error.SetError(ErrorMessage.ConflictPut, 409);
        }

        var exist = await cropRepository.DoesCropExistByCropIdAsync(crop);
        if (!exist)
        {
            logger.LogWarning("Crop with ID {CropId} not found.", crop.CropId);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        var ok = await cropRepository.UpdateCropAsync(crop);
        if (!ok)
        {
            logger.LogError("Failed to update crop with ID {CropId}", crop.CropId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Crop with ID {CropId} updated successfully.", crop.CropId);

        return Error.SetSuccess();
    }
}
