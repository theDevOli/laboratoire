using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class PermissionAdderService
(
    IPermissionRepository permissionRepository,
    ILogger<PermissionAdderService> logger
)
: IPermissionAdderService
{
    public async Task<Error> AddPermissionAsync(PermissionDtoAdd permissionDto)
    {
        logger.LogInformation("Attempting to add permission for RoleId: {RoleId}.", permissionDto.RoleId);

        var permission = permissionDto.ToPermission();

        var exists = await permissionRepository.DoesPermissionExistByRoleIdAsync(permission);
        if (exists)
        {
            logger.LogWarning("Permission already exists for RoleId: {RoleId}.", permission.RoleId);
            return Error.SetError(ErrorMessage.ConflictPost, 409);
        }

        var ok = await permissionRepository.AddPermissionAsync(permission);
        if (!ok)
        {
            logger.LogError("Failed to insert permission into the database for RoleId: {RoleId}", permission.RoleId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Permission added successfully for RoleId: {RoleId}", permission.RoleId);
        return Error.SetSuccess();
    }
}
