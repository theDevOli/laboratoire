
using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Laboratoire.Test.Integration.Repositories;

public class AuthRepositoryIntegrationTest
{
    private readonly AuthRepository _repository;
    private readonly UserRepository _userRepository;
    private readonly NpgsqlConnection _connection;

    public AuthRepositoryIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        var dbContext = new DataContext(config);
        var connectionString = config.GetConnectionString("DefaultConnectionDev");

        _connection = new NpgsqlConnection(connectionString);
        _repository = new AuthRepository(dbContext);
        _userRepository = new UserRepository(dbContext);
    }

    [Fact]
    public async Task GetAuthByUserIdAsync_ShouldSucceed()
    {
        // Arrange
        var user = (await _userRepository.GetAllUsersAsync()).FirstOrDefault();

        // Act
        var result = await _repository.GetAuthByUserIdAsync(user?.UserId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Auth>(result);
    }

    [Fact]
    public async Task DoesAuthExistsAsync_ShouldSucceed()
    {
        // Arrange
        var user = (await _userRepository.GetAllUsersAsync()).FirstOrDefault();

        // Act
        var result = await _repository.DoesAuthExistsAsync(user?.UserId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateAuthAsync_ShouldSucceed()
    {
        // Arrange
        var newUser = new User()
        {
            UserId = default,
            RoleId = 1,
            Username = "Test",
            IsActive = false
        };

        var newUserId = await _userRepository.AddUserAsync(newUser);

        var newAuth = new Auth()
        {
            UserId = newUserId,
            PasswordSalt = [],
            PasswordHash = []
        };

        await _repository.AddAuthAsync(newAuth);

        var toUpdateAuth = new Auth()
        {
            UserId = newUserId,
            PasswordSalt = [010],
            PasswordHash = [110]
        };
        // Act
        var result = await _repository.UpdateAuthAsync(toUpdateAuth);
        var updatedAuth = await _repository.GetAuthByUserIdAsync(newUserId);

        // Assert
        Assert.True(result);
        Assert.Equal(toUpdateAuth.UserId, updatedAuth?.UserId);
        Assert.Equal(toUpdateAuth.PasswordSalt, updatedAuth?.PasswordSalt);
        Assert.Equal(toUpdateAuth.PasswordHash, updatedAuth?.PasswordHash);

        // Tear down
        await _connection.ExecuteAsync
        (
            """
            DELETE FROM users.auth
            WHERE user_id = @UserId;
            """,
            new
            {
                UserId = newUserId
            }
        );
        await _connection.ExecuteAsync
        (
            """
            DELETE FROM users."user"
            WHERE user_id = @UserId;
            """,
            new
            {
                UserId = newUserId
            }
        );
    }

    [Fact]
    public async Task AddAuthAsync_ShouldSucceed()
    {
        // Arrange
        var newUser = new User()
        {
            UserId = default,
            RoleId = 1,
            Username = "Test",
            IsActive = false
        };

        var newUserId = await _userRepository.AddUserAsync(newUser);

        var newAuth = new Auth()
        {
            UserId = newUserId,
            PasswordSalt = [],
            PasswordHash = []
        };

        // Act
        var result = await _repository.AddAuthAsync(newAuth);

        // Assert
        Assert.True(result);

        // Tear down
        await _connection.ExecuteAsync
        (
            """
            DELETE FROM users.auth
            WHERE user_id = @UserId;
            """,
            new
            {
                UserId = newUserId
            }
        );
        await _connection.ExecuteAsync
        (
            """
            DELETE FROM users."user"
            WHERE user_id = @UserId;
            """,
            new
            {
                UserId = newUserId
            }
        );
    }
}
