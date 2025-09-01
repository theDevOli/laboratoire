using System.Security.Cryptography;
using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class AuthChangePasswordService
(
    IAuthRepository authRepository,
    PasswordHasher passwordHasher,
    ILogger<AuthChangePasswordService> logger
)
: IAuthChangePasswordService
{
    public async Task<Error> ChangeUserPasswordAsync(UserDtoChangePassword userDto)
    {
        logger.LogInformation("Change password initiated for user {UserId}", userDto.UserId);
        var auth = await authRepository.GetAuthByUserIdAsync(userDto.UserId);
        if (auth is null)
        {
            logger.LogError("UserId {UserId} not found", userDto.UserId);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        var oldPassword = userDto.OldPassword;
        var newPassword = userDto.UserPassword;

        var passwordSaltFromDb = auth.PasswordSalt;
        var passwordHashFromDb = auth.PasswordHash;

        var oldPasswordHash = passwordHasher.HashPassword(oldPassword!, passwordSaltFromDb!);

        if (!passwordHashFromDb!.SequenceEqual(oldPasswordHash))
        {
            logger.LogError("The old password is wrong for user {UserId}", userDto.UserId);
            return Error.SetError(ErrorMessage.Unauthorized, 401);
        }

        byte[] newPasswordSalt = new byte[128 / 8];

        using (RandomNumberGenerator random = RandomNumberGenerator.Create())
        {
            random.GetNonZeroBytes(newPasswordSalt);
            var passwordHash = passwordHasher.HashPassword(newPassword!, newPasswordSalt);

            auth.PasswordSalt = newPasswordSalt;
            auth.PasswordHash = passwordHash;

            var ok = await authRepository.UpdateAuthAsync(auth);
            if (!ok)
            {
                logger.LogError("Database update failed for user {UserId}", userDto.UserId);
                return Error.SetError(ErrorMessage.DbError, 500);
            }
        }

        logger.LogInformation("Password changed successfully for user {UserId}", userDto.UserId);
        return Error.SetSuccess();
    }
}
