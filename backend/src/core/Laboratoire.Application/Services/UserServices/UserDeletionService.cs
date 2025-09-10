using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.UserServices;

public class UserDeletionService
(
    IUserRepository userRepository,
    ILogger<UserDeletionService> logger
)
: IUserDeletionService
{
    public async Task<Error> DeletionUserAsync(User user)
    {
        logger.LogInformation("Start DeletionService for user with ID: {UserId}", user.UserId);
        var exists = await userRepository.DoesUserExistByIdAsync(user);
        if (!exists)
        {
            logger.LogError("User with ID: {UserId} was not found on the database!", user.UserId);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        var isDeleted = await userRepository.DeleteUserAsync(user.UserId);
        if (!isDeleted)
            return Error.SetError(ErrorMessage.DbError, 500);

        logger.LogInformation("User with ID: {UserId} was deleted from database!", user.UserId);
        return Error.SetSuccess();
    }
}
