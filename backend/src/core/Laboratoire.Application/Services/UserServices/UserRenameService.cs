using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.UserServices;

public class UserRenameService
(
    IUserRepository userRepository,
    ILogger<UserRenameService> logger
)
: IUserRenameService
{
    public async Task<Error> UserRenameAsync(UserDtoRename userDto)
    {
        var user = userDto.ToUser();

        logger.LogInformation("Attempting to rename user with ID: {UserId}", user.UserId);


        var exists = await userRepository.DoesUserExistByIdAsync(user);
        if (!exists)
        {
            logger.LogWarning("User with ID {UserId} not found.", user.UserId);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        var ok = await userRepository.UserRenameAsync(user);
        if (!ok)
        {
            logger.LogError("Failed to rename user with ID {UserId}.", user.UserId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("User with ID {UserId} renamed successfully.", user.UserId);
        return Error.SetSuccess();
    }
}
