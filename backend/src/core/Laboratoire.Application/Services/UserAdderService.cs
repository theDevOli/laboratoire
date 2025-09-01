using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class UserAdderService
(
    IUserRepository userRepository,
    IAuthRegistrationService authRegistrationService,
    ILogger<UserAdderService> logger
)
: IUserAdderService
{
    public async Task<Guid?> AddUserAsync(UserDtoAdd userDto)
    {
        logger.LogInformation("Starting user creation process for: {Username}", userDto.Username);
        var user = userDto.ToUser();

        if (user.RoleId == 4)
        {
            var username = await userRepository.SetUserNameAsync(user.Username);
            user.Username = username;
        }

        logger.LogInformation("Adding user to the database.");
        var userId = await userRepository.AddUserAsync(user);

        var userRegistration = new UserRegistration()
        {
            UserId = userId,
            UserPassword = Constants.DEFAULT_PASSWORD
        };

        logger.LogInformation("Registering authentication credentials for user ID: {UserId}", userId);
        var error = await authRegistrationService.RegisterUserAsync(userRegistration);
        if (error.IsNotSuccess())
        {
            logger.LogError("Authentication registration failed for user ID: {UserId}", userId);
            return null;
        }

        logger.LogInformation("User successfully added and registered with ID: {UserId}", userId);
        return userId;
    }
}
