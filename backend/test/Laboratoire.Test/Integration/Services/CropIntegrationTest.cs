using System.Text.Json;
using Dapper;
using Laboratoire.Application.Services.CropServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;

namespace Laboratoire.Test.Integration.Services;

public class CropIntegrationTest
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;
    private readonly DataContext _dbContext;

    public CropIntegrationTest()
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
    public async Task UpdateCrop_ShouldSucceed()
    {
        // Arrange
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var repository = new CropRepository(_dbContext);

        var service = new CropUpdatableService(repository, NullLogger<CropUpdatableService>.Instance);

        CropParameter cropParameter = new() { Min = 1, Med = 1, Max = 1 };
        string jsonCropParameter = JsonSerializer.Serialize(cropParameter);


        int cropId = await connection.ExecuteScalarAsync<int>
        (
        """
        INSERT INTO document.crop(
            crop_name,
            nitrogen_cover,
            nitrogen_foundation,
            phosphorus,
            potassium,
            min_v
        )
        VALUES
        (
            @CropName,
            @NitrogenCover,
            @NitrogenFoundation,
            @Phosphorus::JSONB,
            @Potassium::JSONB,
            @MinV
        )
        RETURNING crop_id;
        """,
        new
        {
            CropName = "Test",
            NitrogenCover = 1,
            NitrogenFoundation = 2,
            Phosphorus = jsonCropParameter,
            Potassium = jsonCropParameter,
            MinV = 50
        }
        );

        Crop toUpdate = new()
        {
            CropId = cropId,
            CropName = "Updated",
            NitrogenCover = 1,
            NitrogenFoundation = 2,
            Phosphorus = cropParameter,
            Potassium = cropParameter,
            MinV = 50
        };

        // Act
        var response = await service.UpdateCropAsync(toUpdate);

        var updated = await repository.GetCropByIdAsync(cropId);

        // Assert
        Assert.NotNull(updated);
        Assert.False(response.IsNotSuccess());
        Assert.Equal(toUpdate.CropName, updated.CropName);

        await connection.ExecuteAsync("DELETE FROM document.crop WHERE crop_id = @cropId", new { cropId});
    }

}
