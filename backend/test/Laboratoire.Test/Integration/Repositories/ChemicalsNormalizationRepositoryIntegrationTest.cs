using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;

namespace Laboratoire.Test.Integration.Repositories;

public class ChemicalsNormalizationRepositoryIntegrationTest
{
    private readonly ChemicalsNormalizationRepository _repository;
    private readonly ChemicalRepository _chemicalRepository;

    public ChemicalsNormalizationRepositoryIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        var dbContext = new DataContext(config);
        var connectionString = config.GetConnectionString("DefaultConnectionDev");

        _repository = new ChemicalsNormalizationRepository(dbContext);
        _chemicalRepository = new ChemicalRepository(dbContext);
    }

    [Fact]
    public async Task GetAllHazardsAsync_ShouldSucceeds()
    {
        // Act
        var result = await _repository.GetAllHazardsAsync();

        // Assert
        Assert.NotEmpty(result);
        Assert.IsAssignableFrom<IEnumerable<ChemicalsNormalization>>(result);
    }

    [Fact]
    public async Task GetHazardsByIdAsync_ShouldSucceeds()
    {
        // Arrange
        var firstNormalization = (await _repository.GetAllHazardsAsync()).FirstOrDefault();

        // Act
        var result = await _repository.GetHazardsByChemicalIdAsync(firstNormalization?.ChemicalId);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.IsAssignableFrom<IEnumerable<ChemicalsNormalization>>(result);
    }

    [Fact]
    public async Task CountHazardAsync_ShouldSucceeds()
    {
        // Arrange
        var firstNormalization = (await _repository.GetAllHazardsAsync()).FirstOrDefault();

        // Act
        var result = await _repository.CountHazardAsync(firstNormalization?.ChemicalId);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task DeleteHazardAsync_ShouldSucceeds()
    {
        // Arrange
        var newChemical = new Chemical()
        {
            ChemicalId = default,
            ChemicalName = "Test",
            Concentration = "Test",
            Quantity = 0,
            Unit = "Test",
            IsPoliceControlled = false,
            IsArmyControlled = false,
            EntryDate = DateTime.Now,
            ExpireDate = DateTime.Now,
        };
        var chemicalId = await _chemicalRepository.AddChemicalAsync(newChemical);

        List<ChemicalsNormalization> newChemicalsNormalization =
        [
            new ChemicalsNormalization()
                    {
                        ChemicalId = chemicalId,
                        HazardId=7
                    }
        ];
        await _repository.AddHazardAsync(newChemicalsNormalization);

        // Act
        var result = await _repository.DeleteHazardAsync(chemicalId);

        // Assert
        Assert.True(result);

        // Tear down
        var chemicalToDelete = new Chemical()
        {
            ChemicalId = chemicalId,
            ChemicalName = "Test",
            Concentration = "Test",
            Quantity = 0,
            Unit = "Test",
            IsPoliceControlled = false,
            IsArmyControlled = false,
            EntryDate = DateTime.Now,
            ExpireDate = DateTime.Now,
        };
        await _chemicalRepository.DeleteChemicalAsync(chemicalToDelete);
    }

    [Fact]
    public async Task AddHazardAsync_ShouldSucceeds()
    {
        // Arrange
        var newChemical = new Chemical()
        {
            ChemicalId = default,
            ChemicalName = "Test",
            Concentration = "Test",
            Quantity = 0,
            Unit = "Test",
            IsPoliceControlled = false,
            IsArmyControlled = false,
            EntryDate = DateTime.Now,
            ExpireDate = DateTime.Now,
        };
        var chemicalId = await _chemicalRepository.AddChemicalAsync(newChemical);

        List<ChemicalsNormalization> newChemicalsNormalization =
        [
            new ChemicalsNormalization()
                    {
                        ChemicalId = chemicalId,
                        HazardId=7
                    }
        ];

        // Act
        var result = await _repository.AddHazardAsync(newChemicalsNormalization);

        // Assert
        Assert.True(result);

        // Tear down
        await _repository.DeleteHazardAsync(chemicalId);
        var chemicalToDelete = new Chemical()
        {
            ChemicalId = chemicalId,
            ChemicalName = "Test",
            Concentration = "Test",
            Quantity = 0,
            Unit = "Test",
            IsPoliceControlled = false,
            IsArmyControlled = false,
            EntryDate = DateTime.Now,
            ExpireDate = DateTime.Now,
        };
        await _chemicalRepository.DeleteChemicalAsync(chemicalToDelete);
    }
}
