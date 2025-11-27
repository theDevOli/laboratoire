using System.Security;
using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Laboratoire.Test.Integration.Repositories;

public class RoleRepositoryIntegrationTest
{
    private readonly RoleRepository _repository;
    private readonly UserRepository _userRepository;
    private readonly NpgsqlConnection _connection;

    public RoleRepositoryIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        var dbContext = new Infrastructure.DbContext.DataContext(config);
        var connectionString = config.GetConnectionString("DefaultConnectionDev");

        _repository = new RoleRepository(dbContext);
        _userRepository = new UserRepository(dbContext);
        _connection = new NpgsqlConnection(connectionString);
    }

    [Fact]
    public async Task GetAllRolesAsync_ShouldSucceed()
    {
        // Act
        var result = await _repository.GetAllRolesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<Role>>(result);
    }

    [Fact]
    public async Task GetRoleByIdAsync_ShouldSucceed()
    {
        // Arrange
        var existingRoleId = (await _repository.GetAllRolesAsync()).First().RoleId;

        // Act
        var result = await _repository.GetRoleByIdAsync(existingRoleId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingRoleId, result?.RoleId);
    }

    [Fact]
    public async Task GetRoleByNameAsync_ShouldSucceed()
    {
        // Arrange
        var existingRoleName = (await _repository.GetAllRolesAsync()).First().RoleName;

        // Act
        var result = await _repository.GetRoleByNameAsync(existingRoleName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingRoleName, result?.RoleName);
    }

    [Fact]
    public async Task GetRoleNameByUserIdAsync_ShouldSucceed()
    {
        // Arrange
        var existingUserId = (await _userRepository.GetAllUsersAsync()).First().UserId;

        // Act
        var result = await _repository.GetRoleNameByUserIdAsync(existingUserId);
        var role = (await _repository.GetAllRolesAsync()).First(r => r.RoleName == result);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(role);

    }

    [Fact]
    public async Task DoesRoleExistByIdAsync_ShouldSucceed()
    {
        // Arrange
        var existingRole = (await _repository.GetAllRolesAsync()).First();

        // Act
        var result = await _repository.DoesRoleExistByIdAsync(existingRole);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DoesRoleExistByNameAsync_ShouldSucceed()
    {
        // Arrange
        var existingRole = (await _repository.GetAllRolesAsync()).First();

        // Act
        var result = await _repository.DoesRoleExistByNameAsync(existingRole);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AddRoleAsync_ShouldSucceed()
    {
        // Arrange
        var roleName = "TestRole";
        var newRole = new Role()
        {
            RoleId = default,
            RoleName = roleName
        };

        // Act
        var result = await _repository.AddRoleAsync(newRole);
        var roleFromDb = await _repository.GetRoleByNameAsync(roleName);

        // Assert
        Assert.True(result);
        Assert.NotNull(roleFromDb);
        Assert.Equal(roleName, roleFromDb?.RoleName);

        // Teardown
        _connection.Open();
        await _connection.ExecuteAsync("DELETE FROM users.role WHERE role_name = @RoleName", new { RoleName = roleName });
    }

    [Fact]
    public async Task UpdateRoleAsync_ShouldSucceed()
    {
        // Arrange
        var roleName = "TestRole";
        var newRole = new Role()
        {
            RoleId = default,
            RoleName = roleName
        };

        await _repository.AddRoleAsync(newRole);

        var roleFromDb = await _repository.GetRoleByNameAsync(roleName);

        var toUpdateRole = new Role()
        {
            RoleId = roleFromDb?.RoleId,
            RoleName = "UpdatedTestRole"
        };


        // Act
        var result = await _repository.UpdateRoleAsync(toUpdateRole);
        var updatedRole = await _repository.GetRoleByIdAsync(toUpdateRole.RoleId);
        // Assert
        Assert.True(result);
        Assert.Equal(toUpdateRole?.RoleId, toUpdateRole?.RoleId);
        Assert.Equal(toUpdateRole?.RoleName, toUpdateRole?.RoleName);

        // Teardown
        _connection.Open();
        await _connection.ExecuteAsync("DELETE FROM users.role WHERE role_name = @RoleName", new { RoleName = roleName });
    }
}
