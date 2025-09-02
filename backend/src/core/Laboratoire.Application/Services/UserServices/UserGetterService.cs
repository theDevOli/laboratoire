using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.UserServices;

public class UserGetterService
(
    IUserRepository userRepository,
    ILogger<UserGetterService> logger
)
: IUserGetterService
{
    public Task<IEnumerable<DisplayUser>> GetAllUsersAsync()
    {
        logger.LogInformation("Fetching all users.");

        return userRepository.GetAllUsersAsync();
    }
}
