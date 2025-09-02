using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.PermissionServices;

public class PermissionGetterService
(
    IPermissionRepository permissionRepository,
    ILogger<PermissionGetterService> logger
)
: IPermissionGetterService
{
    public Task<IEnumerable<DisplayPermission>> GetAllPermissionsAsync()
    {
        logger.LogInformation("Fetching all permissions from the repository.");

        return permissionRepository.GetAllPermissionsAsync();
    }
}
