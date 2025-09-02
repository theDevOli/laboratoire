using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.UserServices;

public class UserPatchService
(
    IUserRepository userRepository,
    ILogger<UserPatchService> logger
)
: IUserPatchService
{
    public async Task<Error> UpdateUserStatusAsync(Guid? userId)
    {
        if (userId is null)
        {
            logger.LogWarning("UpdateUserStatusAsync called with null userId.");
            return Error.SetError(ErrorMessage.BadRequest, 400);
        }
        logger.LogInformation("Fetching user with ID: {UserId} for status update.", userId);

        var user = await userRepository.GetUserByIdAsync(userId);
        if (user is null)
        {
            logger.LogWarning("User not found with ID: {UserId}", userId);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        user.IsActive = !user.IsActive;
        logger.LogInformation("Toggling user status. New IsActive value: {IsActive}", user.IsActive);

        var ok = await userRepository.UpdateUserStatusAsync(user);
        if (!ok)
        {
            logger.LogError("Failed to update user status for ID: {UserId}", userId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("User status updated successfully for ID: {UserId}", userId);
        return Error.SetSuccess();
    }
}
