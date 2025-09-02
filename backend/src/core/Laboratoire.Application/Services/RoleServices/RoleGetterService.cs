using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.RoleServices;

public class RoleGetterService
(
    IRoleRepository roleRepository,
    ILogger<RoleGetterService> logger
)
: IRoleGetterService
{
    public Task<IEnumerable<Role>> GetAllRolesAsync()
    {
        logger.LogInformation("Fetching all roles.");
        return roleRepository.GetAllRolesAsync();
    }
}
