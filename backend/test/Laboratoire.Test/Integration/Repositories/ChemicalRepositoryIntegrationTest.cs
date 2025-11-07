using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Laboratoire.Test.Integration.Repositories;

public class ChemicalRepositoryIntegrationTest
{
    private readonly ChemicalRepository _repository;
    private readonly NpgsqlConnection _connection;

    public ChemicalRepositoryIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        var dbContext = new DataContext(config);
        var connectionString = config.GetConnectionString("DefaultConnectionDev");

        _connection = new NpgsqlConnection(connectionString);
        _repository = new ChemicalRepository(dbContext);
    }

    [Fact]
    public async Task GetAllChemicalsAsync_ShouldSucceed()
    {
        // Act
        var result = await _repository.GetAllChemicalsAsync();

        // Assert
        Assert.NotEmpty(result);
        Assert.IsAssignableFrom<IEnumerable<Chemical>>(result);
    }

    [Fact]
    public async Task GetChemicalByIdAsync_ShouldSucceed()
    {
        // Arrange
        var firstChemical = (await _repository.GetAllChemicalsAsync()).FirstOrDefault();

        // Act
        var result = await _repository.GetChemicalByIdAsync(firstChemical?.ChemicalId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Chemical>(result);
    }

    [Fact]
    public async Task GetChemicalByNameAndConcentrationAsync_ShouldSucceed()
    {
        // Arrange
        var firstChemical = (await _repository.GetAllChemicalsAsync()).FirstOrDefault();

        // Act
        var result = await _repository.GetChemicalByNameAndConcentrationAsync(firstChemical!);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Chemical>(result);
    }

    [Fact]
    public async Task DoesChemicalExistByIdAsync_ShouldSucceed()
    {
        // Arrange
        var firstChemical = (await _repository.GetAllChemicalsAsync()).FirstOrDefault();

        // Act
        var result = await _repository.DoesChemicalExistByIdAsync(firstChemical!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DoesChemicalExistByNameAndConcentrationAsync_ShouldSucceed()
    {
        // Arrange
        var firstChemical = (await _repository.GetAllChemicalsAsync()).FirstOrDefault();

        // Act
        var result = await _repository.DoesChemicalExistByNameAndConcentrationAsync(firstChemical!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AddChemicalAsync_ShouldSucceed()
    {
        // Arrange
        var now = DateTime.Now;

        var newChemical = new Chemical()
        {
            ChemicalId = default,
            ChemicalName = "Test",
            Concentration = "Test",
            Quantity = 0,
            Unit = "Test",
            IsPoliceControlled = false,
            IsArmyControlled = false,
            EntryDate = now,
            ExpireDate = now,
        };

        // Act
        var chemicalId = await _repository.AddChemicalAsync(newChemical);
        var chemicalAdded = await _repository.GetChemicalByIdAsync(chemicalId);

        // Assert
        Assert.NotNull(chemicalAdded);
        Assert.IsType<Chemical>(chemicalAdded);

        // Tear down
        await _connection.ExecuteAsync
        (
            """
            DELETE FROM inventory.chemical
            WHERE 
                chemical_name = @ChemicalName
                AND concentration = @Concentration
                AND unit = @Unit;
            """,
            new
            {
                ChemicalName = newChemical.ChemicalName,
                Concentration = newChemical.Concentration,
                Unit = newChemical.Unit,
            }
        );
    }

    [Fact]
    public async Task UpdateChemicalAsync_ShouldSucceed()
    {
        // Arrange
        var now = DateTime.Now;
        var tomorrow = now.AddDays(1);

        var newChemical = new Chemical()
        {
            ChemicalId = default,
            ChemicalName = "Test",
            Concentration = "Test",
            Quantity = 0,
            Unit = "Test",
            IsPoliceControlled = false,
            IsArmyControlled = false,
            EntryDate = now,
            ExpireDate = now,
        };

        var chemicalId = await _repository.AddChemicalAsync(newChemical);

        var toUpdateChemical = new Chemical()
        {
            ChemicalId = chemicalId,
            ChemicalName = "Updated",
            Concentration = "Updated",
            Quantity = 10,
            Unit = "Updated",
            IsPoliceControlled = true,
            IsArmyControlled = true,
            EntryDate = tomorrow,
            ExpireDate = tomorrow,
        };

        // Act
        var result = await _repository.UpdateChemicalAsync(toUpdateChemical);
        var updatedChemical = await _repository.GetChemicalByIdAsync(chemicalId);

        // Assert
        Assert.True(result);
        Assert.NotNull(updatedChemical);
        Assert.Equal(updatedChemical.ChemicalId,toUpdateChemical.ChemicalId);
        Assert.Equal(updatedChemical.ChemicalName,toUpdateChemical.ChemicalName);
        Assert.Equal(updatedChemical.Concentration,toUpdateChemical.Concentration);
        Assert.Equal(updatedChemical.Quantity,toUpdateChemical.Quantity);
        Assert.Equal(updatedChemical.Unit,toUpdateChemical.Unit);
        Assert.Equal(updatedChemical.IsPoliceControlled,toUpdateChemical.IsPoliceControlled);
        Assert.Equal(updatedChemical.IsArmyControlled,toUpdateChemical.IsArmyControlled);
        Assert.Equal(updatedChemical.EntryDate!.Value.Date,toUpdateChemical.EntryDate.Value.Date);
        Assert.Equal(updatedChemical.ExpireDate!.Value.Date,toUpdateChemical.ExpireDate.Value.Date);
        Assert.IsType<Chemical>(updatedChemical);

        // Tear down
                // Tear down
        await _connection.ExecuteAsync
        (
            """
            DELETE FROM inventory.chemical
            WHERE 
                chemical_name = @ChemicalName
                AND concentration = @Concentration
                AND unit = @Unit;
            """,
            new
            {
                ChemicalName = toUpdateChemical.ChemicalName,
                Concentration = toUpdateChemical.Concentration,
                Unit = toUpdateChemical.Unit,
            }
        );
    }

    [Fact]
    public async Task DeleteChemicalAsync_ShouldSucceed()
    {
        // Arrange
        var now = DateTime.Now;

        var newChemical = new Chemical()
        {
            ChemicalId = default,
            ChemicalName = "Test",
            Concentration = "Test",
            Quantity = 0,
            Unit = "Test",
            IsPoliceControlled = false,
            IsArmyControlled = false,
            EntryDate = now,
            ExpireDate = now,
        };

        var chemicalId = await _repository.AddChemicalAsync(newChemical);

        var toDeleteChemical = new Chemical()
        {
            ChemicalId = chemicalId,
            ChemicalName = "Test",
            Concentration = "Test",
            Quantity = 0,
            Unit = "Test",
            IsPoliceControlled = false,
            IsArmyControlled = false,
            EntryDate = now,
            ExpireDate = now,
        };

        // Act
        var result = await _repository.DeleteChemicalAsync(toDeleteChemical);

        // Assert
        Assert.True(result);
    }
}
