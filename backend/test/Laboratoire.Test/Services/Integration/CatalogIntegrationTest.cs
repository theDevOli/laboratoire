
using System.Text.Json;
using Dapper;
using Laboratoire.Application.Services.CatalogServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;

namespace Laboratoire.Test.Services.Integration;

public class CatalogIntegrationTest
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;
    private readonly DataContext _dbContext;

    public CatalogIntegrationTest()
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
    public async Task UpdateCatalog_ShouldSucceed()
    {
        // Arrange
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        var repository = new CatalogRepository(_dbContext);
        var service = new CatalogUpdatableService(repository, NullLogger<CatalogUpdatableService>.Instance);
        var legends = new Legend[] { new() { Unit = "Test", Description = "Test" } };
        var strLegends = legends
                                .Select(l => JsonSerializer.Serialize(l))
                                .ToArray();

        var catalogId = await connection.ExecuteScalarAsync<int>
        (
        """
        INSERT INTO parameters.catalog(
            report_type,
            sample_type,
            label_name,
            legends,
            price
        )
        VALUES(
            @ReportType,
            @SampleType,
            @LabelName,
            @Legends::JSONB[],
            @Price
        )
        RETURNING catalog_id;
        """,
        new { ReportType = "Test", SampleType = "Test", LabelName = "Test", Legends = strLegends, Price = 25.88m }
        );

        Catalog toUpdate = new() { CatalogId = catalogId, ReportType = "Updated",SampleType = "Test", LabelName = "Test", Legends = legends, Price = 25.88m  };

        // Act
        var result = await service.UpdateCatalogAsync(toUpdate);
        var updated = await repository.GetCatalogByIdAsync(catalogId);

        // Assert
        Assert.False(result.IsNotSuccess());
        Assert.NotNull(updated);
        Assert.Equal(toUpdate.ReportType, updated.ReportType);

        await connection.ExecuteAsync("DELETE FROM parameters.catalog WHERE catalog_id = @catalogId", new { catalogId });
    }
}
