using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Laboratoire.Test.Integration.Repositories;

public class CropsNormalizationRepositoryIntegrationTest
{
    private readonly CropsNormalizationRepository _repository;
    private readonly ReportRepository _reportRepository;
    private readonly NpgsqlConnection _connection;

    public CropsNormalizationRepositoryIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        var dbContext = new Infrastructure.DbContext.DataContext(config);
        var connectionString = config.GetConnectionString("DefaultConnectionDev");

        _repository = new CropsNormalizationRepository(dbContext);
        _reportRepository = new ReportRepository(dbContext);
        _connection = new NpgsqlConnection(connectionString);
    }

    [Fact]
    public async Task GetAllCropsAsync_ShouldSucceed()
    {
        // Act
        var result = await _repository.GetAllCropsAsync();

        // Assert
        Assert.NotEmpty(result);
        Assert.IsAssignableFrom<IEnumerable<CropsNormalization>>(result);
    }

    [Fact]
    public async Task GetCropByReportIdAsync_ShouldSucceed()
    {
        // Arrange
        var firstReport = (await _reportRepository.GetAllReportsAsync()).FirstOrDefault();

        // Act
        var result = await _repository.GetCropByReportIdAsync(firstReport?.ReportId);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.IsAssignableFrom<IEnumerable<CropsNormalization>>(result);
    }

    [Fact]
    public async Task IsThereNoneCropsAsync_ShouldSucceed()
    {
        // Arrange
        var firstReport = (await _repository.GetAllCropsAsync()).FirstOrDefault();

        // Act
        var result = await _repository.IsThereNoneCropsAsync(firstReport?.ProtocolId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AddCropsAsync_ShouldSucceed()
    {
        // Arrange
        var protocolId = "0001/2025";
        var cropId = 1;
        var newCropsNormalization = new List<CropsNormalization>()
        {
             new() { ProtocolId=protocolId,CropId=cropId}
        };

        // Act
        var result = await _repository.AddCropsAsync(newCropsNormalization);

        // Assert
        Assert.True(result);

        // Tear down

        await _connection.ExecuteAsync
        (
            $"""
            DELETE FROM 
                document.crop_protocol
            WHERE 
                protocol_id = @protocolId
                AND crop_id = @cropId;
            """,
            new
            {
                protocolId,
                cropId
            }
        );
    }

    [Fact]
    public async Task DeleteCropsAsync_ShouldSucceed()
    {
        // Arrange
        var protocolId = "0001/2025";
        var cropId = 1;
        var newCropsNormalization = new List<CropsNormalization>()
        {
             new() { ProtocolId=protocolId,CropId=cropId}
        };
        await _repository.AddCropsAsync(newCropsNormalization);

        // Act
        var result = await _repository.DeleteCropsAsync(protocolId);

        // Assert
        Assert.True(result);
    }

}
