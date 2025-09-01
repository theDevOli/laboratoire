using System;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class UserGetterByUsernameService
(
    IUserRepository userRepository,
    ILogger<UserGetterByUsernameService> logger
)
: IUserGetterByUsernameService
{
    public async Task<User?> GetUserByUsernameAsync(string? username)
    {
        if (username is null)
        {
            logger.LogWarning("GetUserByUsernameAsync was called with a null username.");
            return null;
        }

        logger.LogInformation("Fetching user with username: {Username}", username);
        return await userRepository.GetUserByUsernameAsync(username);
    }
}
