using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;

namespace Laboratoire.Test.Integration.Repositories;

public class UtilsRepositoryIntegrationTest
{
    private readonly UtilsRepository _repository;

    public UtilsRepositoryIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        var dbContext = new Infrastructure.DbContext.DataContext(config);

        _repository = new UtilsRepository(dbContext);
    }

    [Fact]
    public async Task GetAllStatesAsync_ShouldSucceed()
    {
        // Act
        var result = await _repository.GetAllStatesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.IsAssignableFrom<IEnumerable<State>>(result);
    }

    [Fact]
    public async Task GetPostalCodeByCityAndStateAsync_ShouldSucceed()
    {
        // Arrange
        var city = "Itabaiana";
        var stateId = (await _repository.GetAllStatesAsync())!.First(s => s.StateName == "Sergipe").StateId;

        // Act
        var result = await _repository.GetPostalCodeByCityAndStateAsync(city,stateId);

        // Assert
        Assert.NotNull(result);
    }
}
