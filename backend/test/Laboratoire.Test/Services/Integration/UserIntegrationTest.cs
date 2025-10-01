using Laboratoire.Application.Services.UserServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace Laboratoire.Test.Services.Integration;

public class UserIntegrationTest
{
    private readonly IConfiguration _config;
    private readonly DataContext _dbContext;

    public UserIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        _dbContext = new DataContext(_config);
    }

    [Fact]
    public async Task UpdateUser_ShouldSucceed()
    {
        // Arrange
        var userRepository = new UserRepository(_dbContext);
        var service = new UserUpdatableService(userRepository, NullLogger<UserUpdatableService>.Instance);
        User user = new()
        {
            Username = "testuser",
            IsActive = true,
            RoleId = 1
        };
        var userId = await userRepository.AddUserAsync(user);

        var toUpdateUser = new User()
        {
            UserId = userId,
            Username = "Updated",
            IsActive = true,
            RoleId = 1
        };

        // Act
        var response = await service.UpdateUserAsync(toUpdateUser);
        var updatedUser = await userRepository.GetUserByIdAsync(userId);

        // Assert
        Assert.False(response.IsNotSuccess());
        Assert.NotNull(updatedUser);
        Assert.Equal(toUpdateUser.Username, updatedUser.Username);

        // Clean up
        await userRepository.DeleteUserAsync(userId);
    }
}
