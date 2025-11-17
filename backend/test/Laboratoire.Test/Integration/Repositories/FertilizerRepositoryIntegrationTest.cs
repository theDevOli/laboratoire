using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Laboratoire.Test.Integration.Repositories;

public class FertilizerRepositoryIntegrationTest
{
    private readonly FertilizerRepository _repository;
    private readonly NpgsqlConnection _connection;

    public FertilizerRepositoryIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        var dbContext = new Infrastructure.DbContext.DataContext(config);
        var connectionString = config.GetConnectionString("DefaultConnectionDev");

        _repository = new FertilizerRepository(dbContext);
        _connection = new NpgsqlConnection(connectionString);
    }

    [Fact]
    public async Task GetAllFertilizersAsync_ShouldSucceed()
    {
        // Act
        var fertilizers = await _repository.GetAllFertilizersAsync();

        // Assert
        Assert.NotEmpty(fertilizers);
        Assert.IsAssignableFrom<IEnumerable<Fertilizer>>(fertilizers);
    }

    [Fact]
    public async Task GetFertilizerByIdAsync_ShouldSucceed()
    {
        // Arrange
        var firstFertilizer = (await _repository.GetAllFertilizersAsync()).FirstOrDefault();

        // Act
        var fertilizer = await _repository.GetFertilizerByIdAsync(firstFertilizer?.FertilizerId);

        // Assert
        Assert.NotNull(fertilizer);
        Assert.IsType<Fertilizer>(fertilizer);
        Assert.Equal(firstFertilizer?.FertilizerId, fertilizer.FertilizerId);
        Assert.Equal(firstFertilizer?.Proportion, fertilizer.Proportion);
        Assert.Equal(firstFertilizer?.IsAvailable, fertilizer.IsAvailable);
        Assert.Equal(firstFertilizer?.Potassium, fertilizer.Potassium);
        Assert.Equal(firstFertilizer?.Nitrogen, fertilizer.Nitrogen);
        Assert.Equal(firstFertilizer?.Phosphorus, fertilizer.Phosphorus);
    }

    [Fact]
    public async Task GetFertilizersByProportionAsync_ShouldSucceed()
    {
        // Arrange
        var firstFertilizer = (await _repository.GetAllFertilizersAsync()).FirstOrDefault();

        // Act
        var fertilizers = await _repository.GetFertilizersByProportionAsync(firstFertilizer?.Proportion);

        // Assert
        Assert.NotEmpty(fertilizers);
        Assert.IsAssignableFrom<IEnumerable<Fertilizer>>(fertilizers);
    }

    [Fact]
    public async Task ChangeFertilizerStatusAsync_ShouldSucceed()
    {
        // Arrange
        var newFertilizer = new Fertilizer()
        {
            FertilizerId = default,
            Nitrogen = 1,
            Phosphorus = 1,
            Potassium = 1,
            IsAvailable = true,
            Proportion = "1-1-1"
        };

        var fertilizerId = await _connection.ExecuteScalarAsync<int>
        (
            """
                INSERT INTO document.fertilizer
                (nitrogen,phosphorus,potassium,is_available)
                VALUES(@Nitrogen,@Phosphorus,@Potassium,@IsAvailable)
                RETURNING fertilizer_id;
            """,
            new
            {
                newFertilizer.Nitrogen,
                newFertilizer.Phosphorus,
                newFertilizer.Potassium,
                newFertilizer.IsAvailable,
            }
        );

        // Act
        var isChanged = await _repository.ChangeFertilizerStatusAsync(fertilizerId);
        var fertilizer = await _repository.GetFertilizerByIdAsync(fertilizerId);

        // Assert
        Assert.True(isChanged);
        Assert.Equal(fertilizerId, fertilizer?.FertilizerId);
        Assert.Equal(newFertilizer?.Proportion, fertilizer?.Proportion);
        Assert.Equal(newFertilizer?.IsAvailable, fertilizer?.IsAvailable);
        Assert.Equal(newFertilizer?.Potassium, fertilizer?.Potassium);
        Assert.Equal(newFertilizer?.Nitrogen, fertilizer?.Nitrogen);
        Assert.Equal(newFertilizer?.Phosphorus, fertilizer?.Phosphorus);

        // Tear down
        await _connection.ExecuteAsync("DELETE FROM document.fertilizer WHERE fertilizer_id = @fertilizerId;", new { fertilizerId });
    }
}
