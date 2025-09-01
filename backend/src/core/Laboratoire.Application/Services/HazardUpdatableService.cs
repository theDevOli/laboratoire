using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class HazardUpdatableService
(
    IHazardRepository hazardRepository,
    ILogger<HazardUpdatableService> logger
)
: IHazardUpdatableService
{
    public async Task<Error> UpdateHazardAsync(Hazard hazard)
    {
        logger.LogInformation("Starting update for hazard ID: {HazardId}, Class: {HazardClass}", hazard.HazardId, hazard.HazardClass);

        var isConflict = await hazardRepository.DoesHazardExistByClassAsync(hazard);
        if (isConflict)
        {
            logger.LogWarning("Conflict detected: Hazard with class '{HazardClass}' already exists.", hazard.HazardClass);
            return Error.SetError(ErrorMessage.ConflictPut, 409);
        }

        var exists = await hazardRepository.DoesHazardExistByIdAsync(hazard);
        if (!exists)
        {
            logger.LogWarning("Hazard with ID {HazardId} not found.", hazard.HazardId);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        var ok = await hazardRepository.UpdateHazardAsync(hazard);
        if (!ok)
        {
            logger.LogError("Failed to update hazard with ID {HazardId}", hazard.HazardId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Hazard with ID {HazardId} updated successfully.", hazard.HazardId);
        return Error.SetSuccess();
    }
}
