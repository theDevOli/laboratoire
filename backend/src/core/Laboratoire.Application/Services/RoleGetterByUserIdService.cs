using System;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class RoleGetterByUserIdService
(
    IRoleRepository roleRepository,
    ILogger<RoleGetterByUserIdService> logger
)
: IRoleGetterByUserIdService
{
    public Task<string?> GetRoleNameByUserIdAsync(Guid? userId)
    {
        logger.LogInformation("Starting GetRoleNameByUserIdAsync with user ID {UserId}.", userId);

        return roleRepository.GetRoleNameByUserIdAsync(userId);
    }
}
