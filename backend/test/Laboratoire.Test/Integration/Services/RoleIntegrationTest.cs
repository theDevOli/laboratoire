using Dapper;
using Laboratoire.Application.Services.RoleServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;

namespace Laboratoire.Test.Integration.Services;

public class RoleIntegrationTest
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;
    private readonly DataContext _dbContext;

    public RoleIntegrationTest()
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
    public async Task UpdateRole_ShouldSucceed()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var repository = new RoleRepository(_dbContext);
        var service = new RoleUpdatableService(repository, NullLogger<RoleUpdatableService>.Instance);

        var roleId = await connection.ExecuteScalarAsync<int>(
            """
            INSERT INTO users.role (role_name)
            VALUES ('TestRole')
            RETURNING role_id;
            """
        );

        var toUpdateRole = new Role
        {
            RoleId = roleId,
            RoleName = "Updated"
        };

        var response = await service.UpdateRoleAsync(toUpdateRole);

        var updatedRole = await repository.GetRoleByIdAsync(roleId);

        Assert.False(response.IsNotSuccess());
        Assert.NotNull(updatedRole);
        Assert.Equal(toUpdateRole.RoleName, updatedRole.RoleName);

        await connection.ExecuteAsync(
            "DELETE FROM users.role WHERE role_id = @RoleId;",
            new { RoleId = roleId }
        );
    }
}
