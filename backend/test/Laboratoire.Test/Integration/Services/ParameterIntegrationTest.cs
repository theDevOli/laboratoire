using Dapper;
using Laboratoire.Application.Services.ParameterServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;


namespace Laboratoire.Test.Services.Integration;

public class ParameterIntegrationTest
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;
    private readonly DataContext _dbContext;

    public ParameterIntegrationTest()
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
    public async Task UpdateParameter_ShouldSucceed()
    {
        // Arrange
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var repository = new ParameterRepository(_dbContext);

        var service = new ParameterUpdatableService(repository, NullLogger<ParameterUpdatableService>.Instance);

        int parameterId = await connection.ExecuteScalarAsync<int>
        (
            """
            INSERT INTO parameters.parameter (
                catalog_id,
                parameter_name,
                unit,
                input_quantity,
                official_doc,
                vmp,
                equation
            )
            VALUES 
            (
                @CatalogId,
                @ParameterName, 
                @Unit,
                @InputQuantity,
                @OfficialDoc,
                @Vmp,
                @Equation
            )
            RETURNING parameter_id;
            """,
            new
            {
                CatalogId = 1,
                ParameterName = "Test",
                Unit = "Test",
                InputQuantity = 1,
                OfficialDoc = "Test",
                Vmp = "Test",
                Equation = "Test",
            }
        );

        Parameter toUpdate = new()
        {
            ParameterId = parameterId,
            CatalogId = 1,
            ParameterName = "Updated",
            Unit = "Test",
            InputQuantity = 1,
            OfficialDoc = "Test",
            Vmp = "Test",
            Equation = "Test",
        };

        // Act
        var response = await service.UpdateParameterAsync(toUpdate);

        var updated = await repository.GetParameterByParameterIdAsync(parameterId);

        // Assert
        Assert.False(response.IsNotSuccess());
        Assert.NotNull(updated);
        Assert.Equal(updated.ParameterName, toUpdate.ParameterName);

        await connection.ExecuteAsync("DELETE FROM parameters.parameter WHERE parameter_id = @parameterId", new { parameterId });
    }
}
