using System.Security.Cryptography;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class AuthResetPasswordService
(
    IAuthRepository authRepository,
    PasswordHasher passwordHasher,
    ILogger<AuthResetPasswordService> logger
)
: IAuthResetPasswordService
{
    public async Task<Error> ResetPasswordAsync(Guid? userId)
    {
        logger.LogInformation("Password reset initiated for userId {UserId}", userId);

        string newPassword = Constants.DEFAULT_PASSWORD;
        var auth = await authRepository.GetAuthByUserIdAsync(userId);
        if (auth is null)
        {
            logger.LogWarning("Reset password failed: UserId is null");
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        byte[] newPasswordSalt = new byte[128 / 8];

        using (RandomNumberGenerator random = RandomNumberGenerator.Create())
        {
            random.GetNonZeroBytes(newPasswordSalt);
            var passwordHash = passwordHasher.HashPassword(newPassword, newPasswordSalt);

            auth.PasswordSalt = newPasswordSalt;
            auth.PasswordHash = passwordHash;

            var ok = await authRepository.UpdateAuthAsync(auth);
            if (!ok)
            {
                logger.LogError("Failed to update auth data for userId {UserId}", userId);
                return Error.SetError(ErrorMessage.DbError, 500);
            }
        }

        logger.LogInformation("Password successfully reset for userId {UserId}", userId);
        return Error.SetSuccess();
    }
}
