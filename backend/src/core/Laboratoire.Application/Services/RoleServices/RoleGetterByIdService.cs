using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.RoleServices;

public class RoleGetterByIdService
(
    IRoleRepository roleRepository,
    ILogger<RoleGetterByIdService> logger
)
: IRoleGetterByIdService
{
    public async Task<Role?> GetRoleByIdAsync(int? roleId)
    {
        if (roleId is null)
        {
            logger.LogWarning("GetRoleByIdAsync called with null roleId.");
            return null;
        }
        
        logger.LogInformation("Fetching role with ID: {RoleId}", roleId);
        return await roleRepository.GetRoleByIdAsync(roleId);
    }
}
