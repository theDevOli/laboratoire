using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Laboratoire.Test.Integration.Repositories;

public class HazardRepositoryIntegrationTest
{
    private readonly HazardRepository _repository;
    private readonly NpgsqlConnection _connection;

    public HazardRepositoryIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        var dbContext = new Infrastructure.DbContext.DataContext(config);
        var connectionString = config.GetConnectionString("DefaultConnectionDev");

        _repository = new HazardRepository(dbContext);
        _connection = new NpgsqlConnection(connectionString);
    }

    [Fact]
    public async Task GetAllHazardsAsync_ShouldSucceed()
    {
        // Act
        var hazards = await _repository.GetAllHazardsAsync();

        // Assert
        Assert.NotEmpty(hazards);
        Assert.IsAssignableFrom<IEnumerable<Hazard>>(hazards);
    }

    [Fact]
    public async Task GetHazardByIdAsync_ShouldSucceed()
    {
        // Arrange
        var firstHazard = (await _repository.GetAllHazardsAsync()).FirstOrDefault();

        // Act
        var hazard = await _repository.GetHazardByIdAsync(firstHazard?.HazardId);

        // Assert
        Assert.NotNull(hazard);
        Assert.IsType<Hazard>(hazard);
    }

    [Fact]
    public async Task GetHazardByClassAsync_ShouldSucceed()
    {
        // Arrange
        var firstHazard = (await _repository.GetAllHazardsAsync()).FirstOrDefault();

        // Act
        var hazard = await _repository.GetHazardByClassAsync(firstHazard?.HazardClass);

        // Assert
        Assert.NotNull(hazard);
        Assert.IsType<Hazard>(hazard);
    }

    [Fact]
    public async Task DoesHazardExistByIdAsync_ShouldSucceed()
    {
        // Arrange
        var firstHazard = (await _repository.GetAllHazardsAsync()).FirstOrDefault();

        // Act
        var doesExist = await _repository.DoesHazardExistByIdAsync(firstHazard!);

        // Assert
        Assert.True(doesExist);
    }

    [Fact]
    public async Task DoesHazardExistByClassAsync_ShouldSucceed()
    {
        // Arrange
        var firstHazard = (await _repository.GetAllHazardsAsync()).FirstOrDefault();

        // Act
        var doesExist = await _repository.DoesHazardExistByClassAsync(firstHazard!);

        // Assert
        Assert.True(doesExist);
    }

    [Fact]
    public async Task AddHazardAsync_ShouldSucceed()
    {
        // Arrange
        var hazard = new Hazard()
        {
            HazardId = default,
            HazardClass = "Test",
            HazardName = "Test",
        };

        // Act
        var isAdded = await _repository.AddHazardAsync(hazard);

        // Assert
        Assert.True(isAdded);

        // Tear down
        await _connection.ExecuteAsync
        (
            """
            DELETE FROM inventory.hazard
            WHERE 
                hazard_class = @HazardClass
                AND hazard_name = @HazardName;
            """,
            new
            {
                HazardClass = hazard.HazardClass,
                HazardName = hazard.HazardName
            }
        );
    }

    [Fact]
    public async Task UpdateHazardAsync_ShouldSucceed()
    {
        // Arrange
        var newHazard = new Hazard()
        {
            HazardId = default,
            HazardClass = "Test",
            HazardName = "Test",
        };

        await _repository.AddHazardAsync(newHazard);

        var toUpdateHazard = await _repository.GetHazardByClassAsync("Test");
        toUpdateHazard!.HazardClass = "Lest";
        toUpdateHazard.HazardName = "Lest";

        // Act
        var isUpdated = await _repository.UpdateHazardAsync(toUpdateHazard);
        var updatedHazard = await _repository.GetHazardByIdAsync(toUpdateHazard.HazardId);

        // Assert
        Assert.True(isUpdated);
        Assert.Equal(toUpdateHazard.HazardId,updatedHazard?.HazardId);
        Assert.Equal(toUpdateHazard.HazardClass,updatedHazard?.HazardClass);
        Assert.Equal(toUpdateHazard.HazardName,updatedHazard?.HazardName);

        // Tear down
        await _connection.ExecuteAsync
        (
            """
            DELETE FROM inventory.hazard
            WHERE 
                hazard_class = @HazardClass
                AND hazard_name = @HazardName;
            """,
            new
            {
                HazardClass = updatedHazard?.HazardClass,
                HazardName = updatedHazard?.HazardName
            }
        );
    }
}
