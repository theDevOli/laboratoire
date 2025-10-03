using Laboratoire.Application.DTO;
using Laboratoire.Application.Services.ChemicalServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace Laboratoire.Test.Integration.Services;

public class ChemicalIntegrationTest
{
    private readonly IConfiguration _config;
    private readonly DataContext _dbContext;

    public ChemicalIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        _dbContext = new DataContext(_config);

    }

    [Fact]
    public async Task UpdateChemical_ShouldSucceed()
    {
        // Arrange
        var repository = new ChemicalRepository(_dbContext);
        var normalizationRepository = new ChemicalsNormalizationRepository(_dbContext);

        var chemicalDeletionNormalizationService =
        new ChemicalsNormalizationDeleterService(normalizationRepository, NullLogger<ChemicalsNormalizationDeleterService>.Instance);

        var chemicalAdderNormalizationService =
        new ChemicalsNormalizationAdderService
        (normalizationRepository, chemicalDeletionNormalizationService, NullLogger<ChemicalsNormalizationAdderService>.Instance);

        var service =
        new ChemicalUpdatableService(repository, chemicalAdderNormalizationService, NullLogger<ChemicalUpdatableService>.Instance);

        var now = DateTime.Now;
        Chemical newChemical = new()
        {
            ChemicalName = "Test",
            Concentration = "0,5",
            Quantity = 2,
            Unit = "Test",
            IsArmyControlled = false,
            IsPoliceControlled = false,
            EntryDate = now,
            ExpireDate = now.AddYears(5)
        };

        var chemicalId = await repository.AddChemicalAsync(chemical: newChemical);
        newChemical.ChemicalName = "Updated";
        var toUpdate = new ChemicalDtoGetUpdate()
        {
            ChemicalId = chemicalId,
            ChemicalName = "Updated",
            Concentration = "0,5",
            Quantity = 2,
            Unit = "Test",
            IsArmyControlled = false,
            IsPoliceControlled = false,
            EntryDate = now,
            ExpireDate = now.AddYears(5)
        };

        // Act
        var response = await service.UpdateChemicalAsync(toUpdate);
        var updated = await repository.GetChemicalByIdAsync(chemicalId);

        // Assert
        Assert.False(response.IsNotSuccess());
        Assert.NotNull(updated);
        Assert.Equal(toUpdate.ChemicalName, updated.ChemicalName);

        await repository.DeleteChemicalAsync(updated);
    }

}
