using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.UserServices;

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
        Guid? userId = null;
        string? customer = null;
        var user = userDto.ToUser();

        if (userDto.Partner is not null)
        {
            logger.LogInformation("Adding user and partner to the database.");

            var username = await userRepository.SetUserNameAsync(user.Username);
            user.Username = username;
            
            var partner = userDto.Partner;

            userId = await userRepository.AddUserAndPartnerAsync(user, partner);
            customer = "partner";
        }

        if (userDto.Client is not null)
        {
            logger.LogInformation("Adding user and client to the database.");
            var client = userDto.Client;
            userId = await userRepository.AddUserAndClientAsync(user, client);
            customer = "client";
        }

        if(userDto.Client is null && userDto.Partner is  null)
            userId = await userRepository.AddUserAndEmployeeAsync(user, userDto.Name);
        
        // logger.LogInformation("Adding user to the database.");
        // var userId = await userRepository.AddUserAsync(user);

        if (userId is null)
        {
            logger.LogError("Add user and {Customer} failed.", customer);
            return null;
        }

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
