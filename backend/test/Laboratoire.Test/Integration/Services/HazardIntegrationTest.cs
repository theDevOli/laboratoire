using Dapper;
using Laboratoire.Application.Services.HazardServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;

namespace Laboratoire.Test.Integration.Services;

public class HazardIntegrationTest
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;
    private readonly DataContext _dbContext;

    public HazardIntegrationTest()
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
    public async Task UpdateHazard_ShouldSucceed()
    {
        // Arrange
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var repository = new HazardRepository(_dbContext);

        var service = new HazardUpdatableService(repository, NullLogger<HazardUpdatableService>.Instance);

        int hazardId = await connection.ExecuteScalarAsync<int>
        (
            """
            INSERT INTO inventory.hazard
                (hazard_class,hazard_name)
            VALUES
                (@HazardClass,@HazardName)
            RETURNING hazard_id;
            """,
            new { HazardClass = "Test", HazardName = "Test" }
        );

        Hazard toUpdate = new() { HazardId = hazardId, HazardClass = "Test", HazardName = "Updated" };

        // Act
        var response = await service.UpdateHazardAsync(toUpdate);

        var updated = await repository.GetHazardByIdAsync(hazardId);

        // Assert
        Assert.NotNull(updated);
        Assert.Equal(0,response.StatusCode);
        Assert.False(response.IsNotSuccess());
        Assert.Equal(updated.HazardName, toUpdate.HazardName);

        await connection.ExecuteAsync("DELETE FROM inventory.hazard WHERE hazard_id = @hazardId", new { hazardId });
    }
}
