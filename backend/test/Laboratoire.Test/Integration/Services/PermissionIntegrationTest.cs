using Dapper;
using Laboratoire.Application.Services.PermissionServices;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;

namespace Laboratoire.Test.Integration.Services;

public class PermissionIntegrationTest
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;
    private readonly DataContext _dbContext;

    public PermissionIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        _connectionString = _config.GetConnectionString("DefaultConnectionDev")!;

        _dbContext = new DataContext(_config);
    }

    [Fact]
    public async Task UpdatePermission_ShouldSucceed()
    {
        // Arrange
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var repository = new PermissionRepository(_dbContext);

        var service = new PermissionUpdatableService(repository, NullLogger<PermissionUpdatableService>.Instance);

        int permissionId = await connection.ExecuteScalarAsync<int>
        (
            """
            WITH new_role AS(
                INSERT INTO users.role(role_name)
                VALUES(@RoleName)
                RETURNING role_id
            )
            INSERT INTO users.permission(
                role_id,
                protocol,
                client,
                property,
                cash_flow,
                partner,
                users,
                chemical
            )
            SELECT
                role_id,
                @Protocol,
                @Client,
                @Property,
                @CashFlow,
                @Partner,
                @Users,
                @Chemical
            FROM
                new_role
            RETURNING permission_id;
            """,
            new
            {
                RoleName = "Test",
                Protocol = false,
                Client = false,
                Property = false,
                CashFlow = false,
                Partner = false,
                Users = false,
                Chemical = false,
            }
        );

        var toUpdatePermission = await repository.GetPermissionByPermissionIdAsync(permissionId);
        toUpdatePermission!.Protocol = false;

        // Act
        var result = await service.UpdatePermissionAsync(toUpdatePermission);

        var updatedPermission = await repository.GetPermissionByPermissionIdAsync(permissionId);

        // Assert
        Assert.False(result.IsNotSuccess());
        Assert.NotNull(updatedPermission);
        Assert.Equal(toUpdatePermission.Protocol, updatedPermission.Protocol);

        // Clean up
        await connection.ExecuteAsync("DELETE FROM users.permission WHERE permission_id = @permissionId", new { permissionId });
        await connection.ExecuteAsync("DELETE FROM users.role WHERE role_id = @roleId", new { roleId = toUpdatePermission.RoleId });
    }
}
