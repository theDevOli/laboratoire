using System;
using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Laboratoire.Test.Integration.Repositories;

public class UserRepositoryIntegrationTest
{
    private readonly UserRepository _repository;
    private readonly NpgsqlConnection _connection;

    public UserRepositoryIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        var dbContext = new Infrastructure.DbContext.DataContext(config);
        var connectionString = config.GetConnectionString("DefaultConnectionDev");

        _repository = new UserRepository(dbContext);
        _connection = new NpgsqlConnection(connectionString);
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldSucceed()
    {
        // Act
        var result = await _repository.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<DisplayUser>>(result);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldSucceed()
    {
        // Arrange
        var existingUserId = (await _repository.GetAllUsersAsync()).First().UserId;

        // Act
        var result = await _repository.GetUserByIdAsync(existingUserId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<User>(result);
    }

    [Fact]
    public async Task GetAuthenticationByUserIdAsync_ShouldSucceed()
    {
        // Arrange
        var existingUserId = (await _repository.GetAllUsersAsync()).First().UserId;

        // Act
        var result = await _repository.GetAuthenticationByUserIdAsync(existingUserId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Authentication>(result);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_ShouldSucceed()
    {
        // Arrange
        var existingUsername = (await _repository.GetAllUsersAsync()).First().Username;

        // Act
        var result = await _repository.GetUserByUsernameAsync(existingUsername);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<User>(result);
    }

    [Fact]
    public async Task DoesUserExistByIdAsync_ShouldSucceed()
    {
        // Arrange
        var existingUsername = (await _repository.GetAllUsersAsync()).First().Username;
        var existingUser = await _repository.GetUserByUsernameAsync(existingUsername);

        // Act
        var result = await _repository.DoesUserExistByIdAsync(existingUser!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DoesUserExistByUsernameAsync_ShouldSucceed()
    {
        // Arrange
        var existingUsername = (await _repository.GetAllUsersAsync()).First().Username;
        var existingUser = await _repository.GetUserByUsernameAsync(existingUsername);

        // Act
        var result = await _repository.DoesUserExistByUsernameAsync(existingUser!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AddUserAsync_ShouldSucceed()
    {
        // Arrange
        var existingUsername = (await _repository.GetAllUsersAsync()).First();
        var newUser = new User
        {
            UserId = default,
            RoleId = existingUsername.RoleId,
            Username = "new_integration_test_user",
            IsActive = true
        };

        // Act
        var userId = await _repository.AddUserAsync(newUser);
        var result = await _repository.GetUserByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result!.UserId);
        Assert.Equal(newUser.RoleId, result!.RoleId);
        Assert.Equal(newUser.Username, result!.Username);
        Assert.Equal(newUser.IsActive, result!.IsActive);

        //Tear down
        await _repository.DeleteUserAsync(userId);
    }

    [Fact]
    public async Task AddUserAndClientAsync_ShouldSucceed()
    {
        // Arrange
        var existingUsername = (await _repository.GetAllUsersAsync()).First();
        var newUser = new User
        {
            UserId = default,
            RoleId = existingUsername.RoleId,
            Username = "new_integration_test_user",
            IsActive = true
        };

        var newClient = new Client
        {
            ClientId = default,
            UserId = default,
            ClientName = "Integration Test Client",
            ClientTaxId = "12345678901",
            ClientEmail = "test@email.com",
            ClientPhone = "12345-6789"
        };

        // Act
        var userId = await _repository.AddUserAndClientAsync(newUser, newClient);
        var result = await _repository.GetUserByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result!.UserId);
        Assert.Equal(newUser.RoleId, result!.RoleId);
        Assert.Equal(newUser.Username, result!.Username);
        Assert.Equal(newUser.IsActive, result!.IsActive);

        //Tear down
        await _connection.OpenAsync();
        var transaction = await _connection.BeginTransactionAsync();

        await _connection.ExecuteAsync("DELETE FROM customers.client WHERE user_id = @UserId", new { UserId = userId }, transaction);
        await _connection.ExecuteAsync("""DELETE FROM users."user" WHERE user_id = @UserId""", new { UserId = userId }, transaction);

        await transaction.CommitAsync();
        await _connection.CloseAsync();
    }

    [Fact]
    public async Task AddUserAndPartnerAsync_ShouldSucceed()
    {
        // Arrange
        var existingUsername = (await _repository.GetAllUsersAsync()).First();
        var newUser = new User
        {
            UserId = default,
            RoleId = existingUsername.RoleId,
            Username = "new_integration_test_user",
            IsActive = true
        };
        await _connection.OpenAsync();
        var officeId = await _connection.ExecuteScalarAsync<Guid>("SELECT office_id FROM customers.office LIMIT 1;");

        var newPartner = new Partner
        {
            PartnerId = default,
            OfficeId = officeId,
            UserId = default,
            PartnerName = "Integration Test Partner",
            PartnerPhone = "12345-6789"
        };

        // Act
        var userId = await _repository.AddUserAndPartnerAsync(newUser, newPartner);
        var result = await _repository.GetUserByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result!.UserId);
        Assert.Equal(newUser.RoleId, result!.RoleId);
        Assert.Equal(newUser.Username, result!.Username);
        Assert.Equal(newUser.IsActive, result!.IsActive);

        //Tear down
        var transaction = await _connection.BeginTransactionAsync();

        await _connection.ExecuteAsync("DELETE FROM customers.partner WHERE user_id = @UserId", new { UserId = userId }, transaction);
        await _connection.ExecuteAsync("""DELETE FROM users."user" WHERE user_id = @UserId""", new { UserId = userId }, transaction);

        await transaction.CommitAsync();
        await _connection.CloseAsync();
    }

    [Fact]
    public async Task AddUserAndEmployeeAsync_ShouldSucceed()
    {
        // Arrange
        var existingUsername = (await _repository.GetAllUsersAsync()).First();
        var newUser = new User
        {
            UserId = default,
            RoleId = existingUsername.RoleId,
            Username = "new_integration_test_user",
            IsActive = true
        };

        var employeeName = "Integration Test Employee";
        // Act
        var userId = await _repository.AddUserAndEmployeeAsync(newUser, employeeName);
        var result = await _repository.GetUserByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result!.UserId);
        Assert.Equal(newUser.RoleId, result!.RoleId);
        Assert.Equal(newUser.Username, result!.Username);
        Assert.Equal(newUser.IsActive, result!.IsActive);

        //Tear down
        await _connection.OpenAsync();
        var transaction = await _connection.BeginTransactionAsync();

        await _connection.ExecuteAsync("DELETE FROM employee.employee WHERE name = @employeeName", new { employeeName }, transaction);
        await _connection.ExecuteAsync("""DELETE FROM users."user" WHERE user_id = @UserId""", new { UserId = userId }, transaction);

        await transaction.CommitAsync();
        await _connection.CloseAsync();
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldSucceed()
    {
        // Arrange
        var existingUsername = (await _repository.GetAllUsersAsync()).First();
        var newUser = new User
        {
            UserId = default,
            RoleId = existingUsername.RoleId,
            Username = "new_integration_test_user",
            IsActive = false
        };

        var userId = await _repository.AddUserAsync(newUser);

        var toUpdateUser = new User
        {
            UserId = userId,
            RoleId = existingUsername.RoleId + 1,
            Username = "Updated_integration_test_user",
            IsActive = true
        };

        // Act
        var result = await _repository.UpdateUserAsync(toUpdateUser);
        var updatedUser = await _repository.GetUserByIdAsync(userId);

        // Assert
        Assert.True(result);
        Assert.Equal(toUpdateUser.UserId, updatedUser!.UserId);
        Assert.Equal(toUpdateUser.RoleId, updatedUser!.RoleId);
        Assert.Equal(toUpdateUser.Username, updatedUser!.Username);
        Assert.Equal(toUpdateUser.IsActive, updatedUser!.IsActive);

        //Tear down
        await _connection.OpenAsync();
        var transaction = await _connection.BeginTransactionAsync();

        await _connection.ExecuteAsync("""DELETE FROM users."user" WHERE user_id = @UserId""", new { UserId = userId }, transaction);

        await transaction.CommitAsync();
        await _connection.CloseAsync();
    }

    [Fact]
    public async Task UserRenameAsync_ShouldSucceed()
    {
        // Arrange
        var existingUsername = (await _repository.GetAllUsersAsync()).First();
        var newUser = new User
        {
            UserId = default,
            RoleId = existingUsername.RoleId,
            Username = "new_integration_test_user",
            IsActive = false
        };

        var userId = await _repository.AddUserAsync(newUser);

        var toUpdateUser = new User
        {
            UserId = userId,
            RoleId = existingUsername.RoleId,
            Username = "Updated_integration_test_user",
            IsActive = false
        };

        // Act
        var result = await _repository.UserRenameAsync(toUpdateUser);
        var updatedUser = await _repository.GetUserByIdAsync(userId);

        // Assert
        Assert.True(result);
        Assert.Equal(toUpdateUser.UserId, updatedUser!.UserId);
        Assert.Equal(toUpdateUser.Username, updatedUser!.Username);

        //Tear down
        await _connection.OpenAsync();

        await _connection.ExecuteAsync("""DELETE FROM users."user" WHERE user_id = @UserId""", new { UserId = userId });

        await _connection.CloseAsync();
    }

    [Fact]
    public async Task UpdateUserStatusAsync_ShouldSucceed()
    {
        // Arrange
        var existingUsername = (await _repository.GetAllUsersAsync()).First();
        var newUser = new User
        {
            UserId = default,
            RoleId = existingUsername.RoleId,
            Username = "new_integration_test_user",
            IsActive = false
        };

        var userId = await _repository.AddUserAsync(newUser);

        var toUpdateUser = new User
        {
            UserId = userId,
            RoleId = existingUsername.RoleId,
            Username = "new_integration_test_user",
            IsActive = true
        };

        // Act
        var result = await _repository.UpdateUserStatusAsync(toUpdateUser);
        var updatedUser = await _repository.GetUserByIdAsync(userId);

        // Assert
        Assert.True(result);
        Assert.Equal(toUpdateUser.UserId, updatedUser!.UserId);
        Assert.Equal(toUpdateUser.IsActive, updatedUser!.IsActive);

        //Tear down
        await _connection.OpenAsync();

        await _connection.ExecuteAsync("""DELETE FROM users."user" WHERE user_id = @UserId""", new { UserId = userId });

        await _connection.CloseAsync();
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldSucceed()
    {
        // Arrange
        var existingUsername = (await _repository.GetAllUsersAsync()).First();
        var newUser = new User
        {
            UserId = default,
            RoleId = existingUsername.RoleId,
            Username = "new_integration_test_user",
            IsActive = false
        };

        var userId = await _repository.AddUserAsync(newUser);


        // Act
        var result = await _repository.DeleteUserAsync(userId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task SetUserNameAsync_ShouldSucceed()
    {
        // Arrange
        var name = "Hugo";

        // Act
        var result = await _repository.SetUserNameAsync(name);

        // Assert
        Assert.Contains(name, result);
    }
}
