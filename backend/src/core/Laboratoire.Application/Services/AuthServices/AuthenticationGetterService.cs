using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.AuthServices;

public class AuthenticationGetterService
(
    IUserRepository userRepository,
    ILogger<AuthenticationGetterService> logger
)
: IAuthenticationGetterService
{
    public Task<Authentication?> GetAuthenticationByUserId(Guid? userId)
    {
        logger.LogInformation("User Authentication initiated for user {UserId}", userId);

        return  userRepository.GetAuthenticationByIdAsync(userId);
    }
}
