using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class RoleAdderService
(
    IRoleRepository roleRepository,
    ILogger<RoleAdderService> logger
)
: IRoleAdderService
{
    public async Task<Error> AddRoleAsync(RoleDtoAdd roleDto)
    {
        var role = roleDto.ToRole();
        logger.LogInformation("Attempting to add role with name: {RoleName}", role.RoleName);


        var exists = await roleRepository.DoesRoleExistByNameAsync(role);
        if (exists)
        {
            logger.LogWarning("Role with name {RoleName} already exists.", role.RoleName);
            return Error.SetError(ErrorMessage.ConflictPost, 409);
        }

        var ok = await roleRepository.AddRoleAsync(role);
        if (!ok)
        {
            logger.LogError("Failed to add role with name {RoleName} to the database.", role.RoleName);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Role with name {RoleName} added successfully.", role.RoleName);
        return Error.SetSuccess();
    }
}