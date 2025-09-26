using Dapper;
using Laboratoire.Application.Services.PermissionServices;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;

namespace Laboratoire.Test.Services.Integration;

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
                RETURNING role_id;
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
            );
            """,
            new
            {
                RoleName="Test"
            }
        );
    }
}
