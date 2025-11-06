using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Laboratoire.Test.Integration.Repositories;

public class CatalogRepositoryIntegrationTest
{
    private readonly CatalogRepository _repository;
    private readonly NpgsqlConnection _connection;

    public CatalogRepositoryIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        var dbContext = new DataContext(config);
        var connectionString = config.GetConnectionString("DefaultConnectionDev");

        _connection = new NpgsqlConnection(connectionString);
        _repository = new CatalogRepository(dbContext);
    }

    [Fact]
    public async Task GetAllCatalogsAsync_ShouldSucceed()
    {
        // Act
        var result = await _repository.GetAllCatalogsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<Catalog>>(result);
    }

    [Fact]
    public async Task GetCatalogByIdAsync_ShouldSucceed()
    {
        var expectedCatalog = (await _repository.GetAllCatalogsAsync()).FirstOrDefault();

        // Act
        var result = await _repository.GetCatalogByIdAsync(expectedCatalog?.CatalogId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Catalog>(result);
        Assert.Equal(expectedCatalog?.CatalogId, result.CatalogId);
    }

    [Fact]
    public async Task GetUniqueCatalogAsync_ShouldSucceed()
    {
        var expectedCatalog = (await _repository.GetAllCatalogsAsync()).FirstOrDefault();

        // Act
        var result = await _repository.GetUniqueCatalogAsync(expectedCatalog!);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Catalog>(result);
        Assert.Equal(expectedCatalog?.CatalogId, result.CatalogId);
    }

    [Fact]
    public async Task DoesCatalogExistByIdAsync_ShouldSucceed()
    {
        var expectedCatalog = (await _repository.GetAllCatalogsAsync()).FirstOrDefault();

        // Act
        var result = await _repository.DoesCatalogExistByIdAsync(expectedCatalog!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DoesCatalogExistByUniqueAsync_ShouldSucceed()
    {
        var expectedCatalog = (await _repository.GetAllCatalogsAsync()).FirstOrDefault();

        // Act
        var result = await _repository.DoesCatalogExistByUniqueAsync(expectedCatalog!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AddCatalogAsync_ShouldSucceed()
    {
        var newCatalog = new Catalog()
        {
            CatalogId = default,
            ReportType = "Test",
            SampleType = "Test",
            LabelName = "Test",
            Legends = [new Legend() { Unit = "Test", Description = "Test" }],
            Price = 20
        };

        // Act
        var result = await _repository.AddCatalogAsync(newCatalog);

        // Assert
        Assert.True(result);

        // Tear down
        await _connection.ExecuteAsync
        (
            """
            DELETE FROM parameters.catalog
            WHERE
                report_type = @ReportType
                AND sample_type = @SampleType
                AND label_name = @LabelName; 
            """,

        new
        {
            ReportType = newCatalog.ReportType,
            SampleType = newCatalog.SampleType,
            LabelName = newCatalog.LabelName,
        }
        );
    }

    [Fact]
    public async Task UpdateCatalogAsync_ShouldSucceed()
    {
        var newCatalog = new Catalog()
        {
            CatalogId = default,
            ReportType = "Test",
            SampleType = "Test",
            LabelName = "Test",
            Legends = [new Legend() { Unit = "Test", Description = "Test" }],
            Price = 0
        };

        await _repository.AddCatalogAsync(newCatalog);

        var catalogId = await _connection.QuerySingleAsync<int>
        (
            """
            SELECT
                catalog_id
            FROM
                parameters.catalog
            WHERE
                report_type = @ReportType
                AND sample_type = @SampleType
                AND label_name = @LabelName; 
            """,
            new
            {
                ReportType = newCatalog.ReportType,
                SampleType = newCatalog.SampleType,
                LabelName = newCatalog.LabelName,
            }
        );

        var toUpdateCatalog = new Catalog()
        {
            CatalogId = catalogId,
            ReportType = "Updated",
            SampleType = "Updated",
            LabelName = "Updated",
            Legends = [new Legend() { Unit = "Updated", Description = "Updated" }],
            Price = 10
        };

        // Act
        var result = await _repository.UpdateCatalogAsync(toUpdateCatalog);
        var updatedCatalog = await _repository.GetCatalogByIdAsync(catalogId);

        // Assert
        Assert.True(result);
        Assert.Equal(toUpdateCatalog.CatalogId, updatedCatalog?.CatalogId);
        Assert.Equal(toUpdateCatalog.ReportType, updatedCatalog?.ReportType);
        Assert.Equal(toUpdateCatalog.SampleType, updatedCatalog?.SampleType);
        Assert.Equal(toUpdateCatalog.LabelName, updatedCatalog?.LabelName);
        Assert.Equal(toUpdateCatalog.Legends, updatedCatalog?.Legends);
        Assert.Equal(toUpdateCatalog.Price, updatedCatalog?.Price);

        // Tear down
        await _connection.ExecuteAsync
        (
            """
            DELETE FROM parameters.catalog
            WHERE
                report_type = @ReportType
                AND sample_type = @SampleType
                AND label_name = @LabelName; 
            """,

        new
        {
            ReportType = toUpdateCatalog.ReportType,
            SampleType = toUpdateCatalog.SampleType,
            LabelName = toUpdateCatalog.LabelName,
        }
        );
    }
}
