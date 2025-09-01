using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class UserGetterByIdService
(
    IUserRepository userRepository,
    ILogger<UserGetterByIdService> logger
)
: IUserGetterByIdService
{
    public async Task<User?> GetUserByIdAsync(Guid? userId)
    {
        if (userId is null)
        {
            logger.LogWarning("GetUserByIdAsync was called with a null userId.");
            return null;
        }

        logger.LogInformation("Fetching user with ID: {UserId}", userId);
        return await userRepository.GetUserByIdAsync(userId);
    }
}
