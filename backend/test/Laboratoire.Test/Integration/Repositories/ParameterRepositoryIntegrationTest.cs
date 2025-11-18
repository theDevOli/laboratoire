using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Laboratoire.Test.Integration.Repositories;

public class ParameterRepositoryIntegrationTest
{
    private readonly ParameterRepository _repository;
    private readonly ReportRepository _reportRepository;
    private readonly NpgsqlConnection _connection;

    public ParameterRepositoryIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        var dbContext = new Infrastructure.DbContext.DataContext(config);
        var connectionString = config.GetConnectionString("DefaultConnectionDev");

        _repository = new ParameterRepository(dbContext);
        _reportRepository = new ReportRepository(dbContext);
        _connection = new NpgsqlConnection(connectionString);
    }

    [Fact]
    public async Task GetAllParametersAsync_ShouldSucceed()
    {
        // Act
        var parameters = await _repository.GetAllParametersAsync();

        // Assert
        Assert.NotEmpty(parameters);
        Assert.IsAssignableFrom<IEnumerable<Parameter>>(parameters);
    }

    [Fact]
    public async Task GetParameterByParameterIdAsync_ShouldSucceed()
    {
        // Arrange
        var firstParameter = (await _repository.GetAllParametersAsync()).FirstOrDefault();

        // Act
        var result = await _repository.GetParameterByParameterIdAsync(firstParameter?.ParameterId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Parameter>(result);
    }

    [Fact]
    public async Task GetParametersByReportIdAsync_ShouldSucceed()
    {
        // Arrange
        var firstReport = (await _reportRepository.GetAllReportsAsync()).FirstOrDefault();

        // Act
        var result = await _repository.GetParametersByReportIdAsync(firstReport?.ReportId);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<Parameter>>(result);
    }

    [Fact]
    public async Task GetParametersInputByCatalogIdAsync_ShouldSucceed()
    {
        // Arrange
        var catalogId = await _connection.QueryFirstAsync<int>("SELECT catalog_id FROM parameters.catalog;");

        // Act
        var result = await _repository.GetParametersInputByCatalogIdAsync<int>(catalogId);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<int>>(result);
    }

    [Fact]
    public async Task GetUniqueParameterAsync_ShouldSucceed()
    {
        // Arrange
        var firstParameter = (await _repository.GetAllParametersAsync()).FirstOrDefault();

        // Act
        var result = await _repository.GetUniqueParameterAsync(firstParameter!);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Parameter>(result);
    }

    [Fact]
    public async Task DoesParameterExistByParameterIdAsync_ShouldSucceed()
    {
        // Arrange
        var firstParameter = (await _repository.GetAllParametersAsync()).FirstOrDefault();

        // Act
        var result = await _repository.DoesParameterExistByParameterIdAsync(firstParameter!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsParameterUniqueAsync_ShouldSucceed()
    {
        // Arrange
        var firstParameter = (await _repository.GetAllParametersAsync()).FirstOrDefault();

        // Act
        var result = await _repository.IsParameterUniqueAsync(firstParameter!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AddParameterAsync_ShouldSucceed()
    {
        // Arrange
        var catalogId = await _connection.QueryFirstAsync<int>("SELECT catalog_id FROM parameters.catalog;");
        var newParameter = new Parameter()
        {
            ParameterId = default,
            CatalogId = catalogId,
            ParameterName = "Test",
            Unit = "Test",
            InputQuantity = 20,
            OfficialDoc = default,
            Vmp = default,
            Equation = default
        };

        // Act
        var isAdded = await _repository.AddParameterAsync(newParameter);

        // Assert
        Assert.True(isAdded);

        // Tear down
        await _connection.ExecuteAsync
        (
            """
                DELETE FROM parameters.parameter
                WHERE 
                    catalog_id = @CatalogId
                    AND parameter_name = @ParameterName
                    AND unit = @Unit
                    AND input_quantity = @InputQuantity;
            """,
            new
            {
                CatalogId = newParameter.CatalogId,
                ParameterName = newParameter.ParameterName,
                Unit = newParameter.Unit,
                InputQuantity = newParameter.InputQuantity,
            }
        );
    }

    [Fact]
    public async Task UpdateParameterAsync_ShouldSucceed()
    {
        // Arrange
        var catalogId = await _connection.QueryFirstAsync<int>("SELECT catalog_id FROM parameters.catalog;");
        var newParameter = new Parameter()
        {
            ParameterId = default,
            CatalogId = catalogId,
            ParameterName = "Test",
            Unit = "Test",
            InputQuantity = 20,
            OfficialDoc = default,
            Vmp = default,
            Equation = default
        };
        await _repository.AddParameterAsync(newParameter);

        var parameterId = await _connection.QueryFirstAsync<int>
        (
            """
                SELECT 
                    parameter_id
                FROM
                    parameters.parameter
                WHERE 
                    catalog_id = @CatalogId
                    AND parameter_name = @ParameterName
                    AND unit = @Unit
                    AND input_quantity = @InputQuantity;
            """,
            new
            {
                CatalogId = newParameter.CatalogId,
                ParameterName = newParameter.ParameterName,
                Unit = newParameter.Unit,
                InputQuantity = newParameter.InputQuantity,
            }
        );

        var toUpdate = new Parameter()
        {
            ParameterId = parameterId,
            CatalogId = catalogId,
            ParameterName = "Lest",
            Unit = "Lest",
            InputQuantity = 0,
            OfficialDoc = default,
            Vmp = default,
            Equation = default
        };

        // Act
        var isUpdated = await _repository.UpdateParameterAsync(toUpdate);
        var updated = await _repository.GetParameterByParameterIdAsync(parameterId);

        // Assert
        Assert.True(isUpdated);
        Assert.Equal(updated?.ParameterId,toUpdate.ParameterId);
        Assert.Equal(updated?.CatalogId,toUpdate.CatalogId);
        Assert.Equal(updated?.ParameterName,toUpdate.ParameterName);
        Assert.Equal(updated?.Unit,toUpdate.Unit);
        Assert.Equal(updated?.InputQuantity,toUpdate.InputQuantity);
        Assert.Equal(updated?.OfficialDoc,toUpdate.OfficialDoc);
        Assert.Equal(updated?.Vmp,toUpdate.Vmp);
        Assert.Equal(updated?.Equation,toUpdate.Equation);

        // Tear down
        await _connection.ExecuteAsync
        (
            """
                DELETE FROM parameters.parameter
                WHERE 
                    catalog_id = @CatalogId
                    AND parameter_name = @ParameterName
                    AND unit = @Unit
                    AND input_quantity = @InputQuantity;
            """,
            new
            {
                CatalogId = newParameter.CatalogId,
                ParameterName = newParameter.ParameterName,
                Unit = newParameter.Unit,
                InputQuantity = newParameter.InputQuantity,
            }
        );
    }
}
