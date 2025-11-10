using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Laboratoire.Test.Integration.Repositories;

public class CropRepositoryIntegrationTest
{
    private readonly CropRepository _repository;
    private readonly NpgsqlConnection _connection;

    public CropRepositoryIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        var dbContext = new Infrastructure.DbContext.DataContext(config);
        var connectionString = config.GetConnectionString("DefaultConnectionDev");

        _repository = new CropRepository(dbContext);
        _connection = new NpgsqlConnection(connectionString);
    }

    [Fact]
    public async Task GetAllCropsAsync_ShouldSucceed()
    {
        // Act
        var result = await _repository.GetAllCropsAsync();

        // Assert
        Assert.NotEmpty(result);
        Assert.IsAssignableFrom<IEnumerable<Crop>>(result);
    }

    [Fact]
    public async Task GetCropByIdAsync_ShouldSucceed()
    {
        // Arrange
        var firstCrop = (await _repository.GetAllCropsAsync()).FirstOrDefault();

        // Act
        var result = await _repository.GetCropByIdAsync(firstCrop?.CropId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Crop>(result);
    }

    [Fact]
    public async Task GetCropByNameAsync_ShouldSucceed()
    {
        // Arrange
        var firstCrop = (await _repository.GetAllCropsAsync()).FirstOrDefault();

        // Act
        var result = await _repository.GetCropByNameAsync(firstCrop?.CropName);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Crop>(result);
    }

    [Fact]
    public async Task DoesCropExistByCropIdAsync_ShouldSucceed()
    {
        // Arrange
        var firstCrop = (await _repository.GetAllCropsAsync()).FirstOrDefault();

        // Act
        var result = await _repository.DoesCropExistByCropIdAsync(firstCrop!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DoesCropExistByNameAsync_ShouldSucceed()
    {
        // Arrange
        var firstCrop = (await _repository.GetAllCropsAsync()).FirstOrDefault();

        // Act
        var result = await _repository.DoesCropExistByNameAsync(firstCrop!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AddCropAsync_ShouldSucceed()
    {
        // Arrange
        var newCrop = new Crop()
        {
            CropId = default,
            CropName = "Test",
            NitrogenCover = 0,
            NitrogenFoundation = 0,
            Phosphorus = new() { Min = 0, Med = 0, Max = 0 },
            Potassium = new() { Min = 0, Med = 0, Max = 0 },
            MinV = 0,
            ExtraData = "Test",
        };

        // Act
        var result = await _repository.AddCropAsync(newCrop);

        // Assert
        Assert.True(result);

        // Tear down
        await _connection.ExecuteAsync
        (
            """
            DELETE FROM document.crop
            WHERE crop_name =@CropName
            AND extra_data = @ExtraData
            """,
            new
            {
                CropName = newCrop.CropName,
                ExtraData = newCrop.ExtraData
            }
        );
    }

    [Fact]
    public async Task UpdateCropAsync_ShouldSucceed()
    {
        // Arrange
        var newCrop = new Crop()
        {
            CropId = default,
            CropName = "Test",
            NitrogenCover = 0,
            NitrogenFoundation = 0,
            Phosphorus = new() { Min = 0, Med = 0, Max = 0 },
            Potassium = new() { Min = 0, Med = 0, Max = 0 },
            MinV = 0,
            ExtraData = "Test",
        };
        await _repository.AddCropAsync(newCrop);

        var cropId = await _connection.QueryFirstOrDefaultAsync<int>
        (
            """
            SELECT
                crop_id
            FROM document.crop
            WHERE crop_name =@CropName
            AND extra_data = @ExtraData
            """,
            new
            {
                CropName = newCrop.CropName,
                ExtraData = newCrop.ExtraData
            }
        );

           var toUpdateCrop = new Crop()
        {
            CropId = cropId,
            CropName = "Updated",
            NitrogenCover = 1,
            NitrogenFoundation = 1,
            Phosphorus = new() { Min = 1, Med = 1, Max = 1 },
            Potassium = new() { Min = 1, Med = 1, Max = 1 },
            MinV = 1,
            ExtraData = "Updated",
        };

        // Act
        var result = await _repository.UpdateCropAsync(toUpdateCrop);
        var updatedCrop = await _repository.GetCropByIdAsync(cropId);

        // Assert
        Assert.True(result);
        Assert.Equal(toUpdateCrop.CropId, updatedCrop?.CropId);
        Assert.Equal(toUpdateCrop.CropName, updatedCrop?.CropName);
        Assert.Equal(toUpdateCrop.NitrogenCover, updatedCrop?.NitrogenCover);
        Assert.Equal(toUpdateCrop.NitrogenFoundation, updatedCrop?.NitrogenFoundation);
        Assert.Equal(toUpdateCrop.Phosphorus, updatedCrop?.Phosphorus);
        Assert.Equal(toUpdateCrop.Potassium, updatedCrop?.Potassium);
        Assert.Equal(toUpdateCrop.MinV, updatedCrop?.MinV);
        Assert.Equal(toUpdateCrop.ExtraData, updatedCrop?.ExtraData);

        // Tear down
        await _connection.ExecuteAsync
        (
            """
            DELETE FROM document.crop
            WHERE crop_name =@CropName
            AND extra_data = @ExtraData
            """,
            new
            {
                CropName = newCrop.CropName,
                ExtraData = newCrop.ExtraData
            }
        );
    }
}
