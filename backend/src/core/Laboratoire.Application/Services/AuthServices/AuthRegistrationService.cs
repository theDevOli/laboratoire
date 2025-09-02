using System.Security.Cryptography;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.AuthServices;

public class AuthRegistrationService
(
    IAuthRepository authRepository,
    PasswordHasher passwordHasher,
    ILogger<AuthRegistrationService> logger
)
: IAuthRegistrationService
{
    public async Task<Error> RegisterUserAsync(UserRegistration user)
    {
        logger.LogInformation("Registration initialized for user: {UserId}", user.UserId);
        var exists = await authRepository.DoesAuthExistsAsync(user.UserId);
        if (exists)
        {
            logger.LogWarning("Registration failed: userId {UserId} already exists", user.UserId);
            return Error.SetError(ErrorMessage.ConflictPost, 409);
        }

        byte[] passwordSalt = new byte[128 / 8];

        using (RandomNumberGenerator random = RandomNumberGenerator.Create())
        {
            random.GetNonZeroBytes(passwordSalt);
            var passwordHash = passwordHasher.HashPassword(user.UserPassword!, passwordSalt);

            var auth = new Auth()
            {
                UserId = user.UserId,
                PasswordSalt = passwordSalt,
                PasswordHash = passwordHash
            };

            var ok = await authRepository.AddAuthAsync(auth);
            if (!ok)
            {
                logger.LogError("Failed to save auth data for userId {UserId}", user.UserId);
                return Error.SetError(ErrorMessage.DbError, 500);
            }
        }

        logger.LogInformation("Registration successfully completed for user: {UserId}", user.UserId);
        return Error.SetSuccess();
    }
}
