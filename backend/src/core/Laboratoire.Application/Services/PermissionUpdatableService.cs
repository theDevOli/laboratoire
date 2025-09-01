using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class PermissionUpdatableService
(
    IPermissionRepository permissionRepository,
    ILogger<PermissionUpdatableService> logger
)
: IPermissionUpdatableService
{
    public async Task<Error> UpdatePermissionAsync(Permission permission)
    {
        logger.LogInformation("Attempting to update permission with ID: {PermissionId}", permission.PermissionId);

        var exists = await permissionRepository.DoesPermissionExistByPermissionIdAsync(permission);
        if (!exists)
        {
            logger.LogWarning("Permission with ID: {PermissionId} not found.", permission.PermissionId);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        var ok = await permissionRepository.UpdatePermissionAsync(permission);
        if (!ok)
        {
            logger.LogError("Failed to update permission with ID: {PermissionId}", permission.PermissionId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Successfully updated permission with ID: {PermissionId}", permission.PermissionId);
        return Error.SetSuccess();
    }
}
