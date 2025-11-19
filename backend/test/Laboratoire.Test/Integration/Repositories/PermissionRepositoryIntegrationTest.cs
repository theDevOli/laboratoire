using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Laboratoire.Test.Integration.Repositories;

public class PermissionRepositoryIntegrationTest
{
    private readonly PermissionRepository _repository;
    private readonly NpgsqlConnection _connection;

    public PermissionRepositoryIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        var dbContext = new Infrastructure.DbContext.DataContext(config);
        var connectionString = config.GetConnectionString("DefaultConnectionDev");

        _repository = new PermissionRepository(dbContext);
        _connection = new NpgsqlConnection(connectionString);
    }

    [Fact]
    public async Task GetAllPermissionsAsync_ShouldSucceed()
    {
        // Act
        var permissions = await _repository.GetAllPermissionsAsync();

        // Assert
        Assert.NotEmpty(permissions);
        Assert.IsAssignableFrom<IEnumerable<DisplayPermission>>(permissions);
    }

    [Fact]
    public async Task GetPermissionByPermissionIdAsync_ShouldSucceed()
    {
        // Arrange
        var firstPermission = (await _repository.GetAllPermissionsAsync()).FirstOrDefault();

        // Act
        var result = await _repository.GetPermissionByPermissionIdAsync(firstPermission?.PermissionId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Permission>(result);
    }

    [Fact]
    public async Task GetPermissionByRoleIdAsync_ShouldSucceed()
    {
        // Arrange
        var firstPermission = (await _repository.GetAllPermissionsAsync()).FirstOrDefault();

        // Act
        var result = await _repository.GetPermissionByRoleIdAsync(firstPermission?.RoleId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Permission>(result);
    }

    [Fact]
    public async Task DoesPermissionExistByPermissionIdAsync_ShouldSucceed()
    {
        // Arrange
        var firstPermission = (await _repository.GetAllPermissionsAsync()).FirstOrDefault();
        var permission = await _repository.GetPermissionByPermissionIdAsync(firstPermission?.PermissionId);

        // Act
        var result = await _repository.DoesPermissionExistByPermissionIdAsync(permission!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DoesPermissionExistByRoleIdAsync_ShouldSucceed()
    {
        // Arrange
        var firstPermission = (await _repository.GetAllPermissionsAsync()).FirstOrDefault();
        var permission = await _repository.GetPermissionByPermissionIdAsync(firstPermission?.PermissionId);

        // Act
        var result = await _repository.DoesPermissionExistByRoleIdAsync(permission!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AddPermissionAsync_ShouldSucceed()
    {
        // Arrange
        var firstPermission = (await _repository.GetAllPermissionsAsync()).FirstOrDefault();
        var newPermission = new Permission()
        {
            PermissionId = default,
            RoleId = firstPermission?.RoleId,
            Protocol = false,
            Client = false,
            Property = false,
            CashFlow = false,
            Partner = false,
            Users = false,
            Chemical = false,
        };

        // Act
        var result = await _repository.AddPermissionAsync(newPermission);

        // Assert
        Assert.True(result);

        //Tear down
        await _connection.ExecuteAsync
        (
            """
            DELETE FROM 
                users.permission 
            WHERE 
                role_id = @RoleId
                AND protocol = false
                AND client = false
                AND property = false
                AND cash_flow = false
                AND partner = false
                AND users = false
                AND chemical = false;
            """,
            new
            {
                RoleId = firstPermission?.RoleId,
            }
        );
    }

    [Fact]
    public async Task UpdatePermissionAsync_ShouldSucceed()
    {
        // Arrange
        var firstPermission = (await _repository.GetAllPermissionsAsync()).FirstOrDefault();
        var newPermission = new Permission()
        {
            PermissionId = default,
            RoleId = firstPermission?.RoleId,
            Protocol = false,
            Client = false,
            Property = false,
            CashFlow = false,
            Partner = false,
            Users = false,
            Chemical = false,
        };
        await _repository.AddPermissionAsync(newPermission);

        var permissionId = await _connection.ExecuteScalarAsync<int>
        (
            """
            SELECT
                permission_id
            FROM 
                users.permission 
            WHERE 
                role_id = @RoleId
                AND protocol = false
                AND client = false
                AND property = false
                AND cash_flow = false
                AND partner = false
                AND users = false
                AND chemical = false;
            """,
            new
            {
                RoleId = firstPermission?.RoleId,
            }
        );

        var toUpdatePermission = new Permission()
        {
            PermissionId = permissionId,
            RoleId = firstPermission?.RoleId,
            Protocol = true,
            Client = true,
            Property = true,
            CashFlow = true,
            Partner = true,
            Users = true,
            Chemical = true,
        };
        // Act
        var result = await _repository.UpdatePermissionAsync(toUpdatePermission);
        var updatedPermission = await _repository.GetPermissionByPermissionIdAsync(permissionId);

        // Assert
        Assert.True(result);
        Assert.Equal(toUpdatePermission.PermissionId,updatedPermission?.PermissionId);
        Assert.Equal(toUpdatePermission.RoleId,updatedPermission?.RoleId);
        Assert.True(updatedPermission?.Protocol);
        Assert.True(updatedPermission?.Client);
        Assert.True(updatedPermission?.Property);
        Assert.True(updatedPermission?.CashFlow);
        Assert.True(updatedPermission?.Partner);
        Assert.True(updatedPermission?.Users);
        Assert.True(updatedPermission?.Chemical);

        //Tear down
        await _connection.ExecuteAsync
        (
            """
            DELETE FROM 
                users.permission 
            WHERE 
                role_id = @RoleId
                AND protocol = false
                AND client = false
                AND property = false
                AND cash_flow = false
                AND partner = false
                AND users = false
                AND chemical = false;
            """,
            new
            {
                RoleId = firstPermission?.RoleId,
            }
        );
    }
}
