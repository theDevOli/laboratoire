using Dapper;
using Laboratoire.Application.Services.TransactionServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;

namespace Laboratoire.Test.Services.Integration;

public class TransactionIntegrationTest
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;
    private readonly DataContext _dbContext;

    public TransactionIntegrationTest()
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
    public async Task UpdateTransaction_ShouldSucceed()
    {
        // Arrange
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var repository = new TransactionRepository(_dbContext);
        var service = new TransactionUpdatableService(repository, NullLogger<TransactionUpdatableService>.Instance);

        var transactionId = await connection.ExecuteScalarAsync<int>(
            """
                INSERT INTO cash_flow.transaction(
                transaction_type,
                owner_name,
                bank_name
            )
            VALUES
            (
                @TransactionType,
                @OwnerName,
                @BankName
            )
            RETURNING transaction_id;
            """,
            new { TransactionType = "TestType", OwnerName = "TestOwner", BankName = "TestBank" }
        );

        var toUpdateTransaction = new Transaction
        {
            TransactionId = transactionId,
            TransactionType = "UpdatedType",
            OwnerName = "TestOwner",
            BankName = "TestBank"
        };

        // Act
        var response = await service.UpdateTransactionAsync(toUpdateTransaction);

        var updatedTransaction = await repository.GetTransactionByIdAsync(transactionId);

        Assert.False(response.IsNotSuccess());
        Assert.NotNull(updatedTransaction);
        Assert.Equal(toUpdateTransaction.TransactionType, updatedTransaction.TransactionType);

        await connection.ExecuteAsync(
            """
            DELETE FROM cash_flow.transaction WHERE transaction_id = @transactionId;
            """,
            new { transactionId }
        );
    }
}
