using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.AuthServices;

public class AuthLoginService
(
    IAuthRepository authRepository,
    PasswordHasher passwordHasher,
    IUserGetterByUsernameService userGetterByUsernameService,
    ILogger<AuthLoginService> logger
)
: IAuthLoginService
{
    private readonly IUserGetterByUsernameService _userGetterByUsernameService = userGetterByUsernameService;
    public async Task<Error> LoginUserAsync(UserLogin userLogin)
    {
        logger.LogInformation("Login initiated for user {Username}.", userLogin.Username);
        var userPassword = userLogin.UserPassword;
        var username = userLogin.Username;
        var user = await _userGetterByUsernameService.GetUserByUsernameAsync(username);
        if (user is null)
        {
            logger.LogError("Login failed: user '{Username}' not found.", userLogin.Username);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        if (user.IsActive == false)
        {
            logger.LogWarning("Login attempt rejected: user '{Username}' is inactive.", userLogin.Username);
            return Error.SetError(ErrorMessage.Forbidden, 403);
        }

        var auth = await authRepository.GetAuthByUserIdAsync(user.UserId);
        if (auth is null)
        {
            logger.LogError("Login failed: user {Username}'s authentication not found.", userLogin.Username);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }
        var passwordSalt = auth.PasswordSalt;
        var passwordHashFromDb = auth.PasswordHash;

        var passwordHashFromLogin = passwordHasher.HashPassword(userPassword!, passwordSalt!);

        if (!passwordHashFromDb!.SequenceEqual(passwordHashFromLogin))
        {
            logger.LogWarning("Login failed: incorrect password for user '{Username}'.", userLogin.Username);
            return Error.SetError(ErrorMessage.Unauthorized, 401);
        }

        logger.LogInformation("Login Successfully completed for user {Username}.", userLogin.Username);
        return Error.SetSuccess();
    }
}
