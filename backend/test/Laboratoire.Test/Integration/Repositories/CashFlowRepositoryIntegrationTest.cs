using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Laboratoire.Test.Integration.Repositories;

public class CashFlowRepositoryIntegrationTest
{
    private readonly CashFlowRepository _repository;
    private readonly PartnerRepository _partnerRepository;
    private readonly NpgsqlConnection _connection;

    public CashFlowRepositoryIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        var dbContext = new DataContext(config);
        var connectionString = config.GetConnectionString("DefaultConnectionDev");

        _connection = new NpgsqlConnection(connectionString);
        _repository = new CashFlowRepository(dbContext);
        _partnerRepository = new PartnerRepository(dbContext);
    }

    [Fact]
    public async Task GetAllCashFlowAsync_ShouldSucceed()
    {
        // Act
        var cashFlows = await _repository.GetAllCashFlowAsync();

        // Assert
        Assert.NotEmpty(cashFlows);
    }

    [Fact]
    public async Task GetCashFlowByYearAndMonthAsync_ShouldSucceed()
    {
        // Arrange
        var year = 2025;
        var month = 1;
        var cashFlows = await _repository.GetAllCashFlowAsync();
        var expectedCashFlow = cashFlows
                                .Where(c => c.PaymentDate!.Value.Year == year && c.PaymentDate.Value.Month == month)
                                .OrderBy(c => c.CashFlowId);

        //  Act
        var tempResult = await _repository.GetCashFlowByYearAndMonthAsync(year, month);

        var result = tempResult.OrderBy(c => c.CashFlowId);

        // Assert
        Assert.Equal(result, expectedCashFlow);
    }

    [Fact]
    public async Task GetCashFlowByIdAsync_ShouldSucceed()
    {
        // Arrange
        var cashFlows = await _repository.GetAllCashFlowAsync();
        var expectedCashFlow = cashFlows.FirstOrDefault();

        //  Act
        var result = await _repository.GetCashFlowByIdAsync(expectedCashFlow!.CashFlowId);

        // Assert
        Assert.Equal(result, expectedCashFlow);
    }

    [Fact]

    public async Task AddCashFlowAsync_ShouldSucceed()
    {
        // Arrange
        var cashFlow = new CashFlow()
        {
            CashFlowId = default,
            TransactionId = 1,
            Description = "AddCashFlowAsync_ShouldSucceed",
            PartnerId = null,
            TotalPaid = null,
            PaymentDate = null,
        };

        //  Act
        var result = await _repository.AddCashFlowAsync(cashFlow);
        var expectedCashFlow = await _connection.QueryFirstOrDefaultAsync<CashFlow>
        (
            $"""
            SELECT 
                cash_flow_id AS {nameof(CashFlow.CashFlowId)},
                transaction_id AS {nameof(CashFlow.TransactionId)},
                description AS {nameof(CashFlow.Description)},
                partner_id AS {nameof(CashFlow.PartnerId)},
                total_paid AS {nameof(CashFlow.TotalPaid)},
                payment_date AS {nameof(CashFlow.PaymentDate)}
            FROM 
                cash_flow.cash_flow
            WHERE
                description = @description;
            """,
            new { description = cashFlow.Description }
        );

        // Assert
        Assert.Equal(cashFlow.Description, expectedCashFlow!.Description);
        Assert.Equal(result, expectedCashFlow.CashFlowId);

        // Tear down
        await _repository.DeleteCashFlowAsync(expectedCashFlow);
        // await _connection.ExecuteAsync
        // (
        //     """
        //     DELETE FROM cash_flow.cash_flow
        //     WHERE cash_flow_id = @cashFlowId
        //     """,
        //     new { cashFlowId = result }
        // );
    }

    [Fact]
    public async Task DoesCashFlowExistsAsync_ShouldSucceed()
    {
        // Arrange
        var cashFlowId = 1;
        var cashFlows = await _repository.GetAllCashFlowAsync();
        var expectedCashFlow = cashFlows.FirstOrDefault(c => c.CashFlowId == cashFlowId);

        //  Act
        var result = await _repository.GetCashFlowByIdAsync(cashFlowId);

        // Assert
        Assert.Equal(result, expectedCashFlow);
    }

    [Fact]
    public async Task UpdateCashFlowAsync_ShouldSucceed()
    {
        // Arrange
        var now = DateTime.Now;
        var tomorrow = now.AddDays(1);

        var partners = (await _partnerRepository.GetAllPartnersAsync()).ToArray();

        var cashFlowToAdd = new CashFlow()
        {
            CashFlowId = default,
            TransactionId = 1,
            Description = "UpdateCashFlowAsync_ShouldSucceed",
            PartnerId = partners[0].PartnerId,
            TotalPaid = 0,
            PaymentDate = now,
        };

        var cashFlowId = await _repository.AddCashFlowAsync(cashFlowToAdd);
        var cashFlowToUpdate = new CashFlow()
        {
            CashFlowId = cashFlowId,
            TransactionId = 2,
            Description = "Updated",
            PartnerId = partners[1].PartnerId,
            TotalPaid = 1,
            PaymentDate = tomorrow,
        };

        //  Act
        var result = await _repository.UpdateCashFlowAsync(cashFlowToUpdate);
        var updatedCashFlow = await _repository.GetCashFlowByIdAsync(cashFlowId);

        // Assert
        Assert.True(result);
        Assert.Equal(cashFlowToUpdate.CashFlowId, updatedCashFlow!.CashFlowId);
        Assert.Equal(cashFlowToUpdate.TransactionId, updatedCashFlow!.TransactionId);
        Assert.Equal(cashFlowToUpdate.Description, updatedCashFlow!.Description);
        Assert.Equal(cashFlowToUpdate.PartnerId, updatedCashFlow!.PartnerId);
        Assert.Equal(cashFlowToUpdate.TotalPaid, updatedCashFlow!.TotalPaid);
        Assert.Equal(cashFlowToUpdate.PaymentDate.Value.Date, updatedCashFlow!.PaymentDate!.Value.Date);

        // Tear down
        await _repository.DeleteCashFlowAsync(cashFlowToUpdate);
    }

    [Fact]
    public async Task DeleteCashFlowAsync_ShouldSucceed()
    {
        // Arrange
        var cashFlow = new CashFlow()
        {
            CashFlowId = default,
            TransactionId = 1,
            Description = "DeleteCashFlowAsync_ShouldSucceed",
            PartnerId = null,
            TotalPaid = null,
            PaymentDate = null,
        };
        var cashFlowId = await _repository.AddCashFlowAsync(cashFlow);
        var cashFlowToDelete = await _repository.GetCashFlowByIdAsync(cashFlowId);

        //  Act
        var result = await _repository.DeleteCashFlowAsync(cashFlowToDelete!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task PatchDescriptionAsync_ShouldSucceed()
    {
        // Arrange
        var cashFlow = new CashFlow()
        {
            CashFlowId = default,
            TransactionId = 1,
            Description = "DeleteCashFlowAsync_ShouldSucceed",
            PartnerId = null,
            TotalPaid = null,
            PaymentDate = null,
        };

        var cashFlowId = await _repository.AddCashFlowAsync(cashFlow);

        var cashFlowToPatch = new CashFlow()
        {
            CashFlowId = cashFlowId,
            TransactionId = 1,
            Description = "Updated",
            PartnerId = null,
            TotalPaid = null,
            PaymentDate = null,
        };

        //  Act
        var result = await _repository.PatchDescriptionAsync(cashFlowToPatch);

        // Assert
        Assert.True(result);

        // Tear down
        await _repository.DeleteCashFlowAsync(cashFlowToPatch);
    }

    [Fact]
    public async Task GetAmountAsync_ShouldSucceed()
    {
        // Arrange
        var year = 2025;
        var month = 1;
        var cashFlowFilter = "in";
        var transaction = 1;

        //  Act
        var result = await _repository.GetAmountAsync(year, month, cashFlowFilter, transaction);

        // Assert
        Assert.NotNull(result);
    }
}
