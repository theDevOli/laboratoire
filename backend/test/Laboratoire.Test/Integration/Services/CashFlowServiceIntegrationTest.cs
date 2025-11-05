using Laboratoire.Application.Services.CashFlowServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;

namespace Laboratoire.Test.Integration.Services;

public class CashFlowServiceIntegrationTest
{
    private readonly IConfiguration _config;
    private readonly DataContext _dbContext;

    public CashFlowServiceIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        _dbContext = new DataContext(_config);
    }
    

    [Fact]
    public async Task UpdateCashFlow_ShouldSucceed()
    {

        // Arrange
        var repository = new CashFlowRepository(_dbContext);
        var service = new CashFlowUpdatableService(repository, NullLogger<CashFlowUpdatableService>.Instance);

        CashFlow cashFlow = new() { TransactionId = 1, Description = "Test" };

        int cashFlowId = await repository.AddCashFlowAsync(cashFlow);

        CashFlow toUpdate = new() { CashFlowId = cashFlowId, TransactionId = cashFlow.TransactionId, Description = "Updated" };

        // Act
        var result = await service.UpdateCashFlowAsync(toUpdate);
        var updated = await repository.GetCashFlowByIdAsync(cashFlowId);

        // Assert
        Assert.False(result.IsNotSuccess());
        Assert.NotNull(updated);
        Assert.Equal(toUpdate.Description, updated.Description);

        // Tears down
        await repository.DeleteCashFlowAsync(updated);
    }

}
