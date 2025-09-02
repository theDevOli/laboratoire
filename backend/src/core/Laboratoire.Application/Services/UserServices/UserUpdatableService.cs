using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.UserServices;

public class UserUpdatableService
(
    IUserRepository userRepository,
    ILogger<UserUpdatableService> logger
)
: IUserUpdatableService
{
    public async Task<Error> UpdateUserAsync(User user)
    {
        logger.LogInformation("Starting update for user with ID: {UserId}", user.UserId);

        var exists = await userRepository.DoesUserExistByIdAsync(user);
        if (!exists)
        {
            logger.LogWarning("User with ID {UserId} not found.", user.UserId);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        var ok = await userRepository.UpdateUserAsync(user);
        if (!ok)
        {
            logger.LogError("Failed to update user with ID {UserId}.", user.UserId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("User with ID {UserId} updated successfully.", user.UserId);
        return Error.SetSuccess();
    }
}
