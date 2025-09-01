using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class RoleUpdatableService
(
    IRoleRepository roleRepository,
    ILogger<RoleUpdatableService> logger
)
: IRoleUpdatableService
{
    public async Task<Error> UpdateRoleAsync(Role role)
    {
        logger.LogInformation("Starting update for role ID: {RoleId}", role.RoleId);

        var exists = await roleRepository.DoesRoleExistByIdAsync(role);
        if (!exists)
        {
            logger.LogWarning("Role with ID {RoleId} not found.", role.RoleId);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        var isConflict = await roleRepository.DoesRoleExistByNameAsync(role);
        if (isConflict)
        {
            logger.LogWarning("Role name conflict detected for name: {RoleName}", role.RoleName);
            return Error.SetError(ErrorMessage.ConflictPut, 409);
        }


        var ok = await roleRepository.UpdateRoleAsync(role);
        if (!ok)
        {
            logger.LogError("Failed to update role with ID {RoleId}.", role.RoleId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Role with ID {RoleId} updated successfully.", role.RoleId);
        return Error.SetSuccess();
    }
}
