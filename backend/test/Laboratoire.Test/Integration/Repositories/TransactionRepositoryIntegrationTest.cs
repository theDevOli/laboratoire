using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Laboratoire.Test.Integration.Repositories;

public class TransactionRepositoryIntegrationTest
{
    private readonly TransactionRepository _repository;
    private readonly NpgsqlConnection _connection;

    public TransactionRepositoryIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        var dbContext = new Infrastructure.DbContext.DataContext(config);
        var connectionString = config.GetConnectionString("DefaultConnectionDev");

        _repository = new TransactionRepository(dbContext);
        _connection = new NpgsqlConnection(connectionString);
    }

    [Fact]
    public async Task GetAllTransactionsAsync_ShouldSucceed()
    {
        // Act
        var transactions = await _repository.GetAllTransactionsAsync();

        // Assert
        Assert.NotNull(transactions);
        Assert.IsAssignableFrom<IEnumerable<Transaction>>(transactions);
    }

    [Fact]
    public async Task GetTransactionByIdAsync_ShouldSucceed()
    {
        // Arrange
        var existingTransactionId = (await _repository.GetAllTransactionsAsync()).First().TransactionId;

        // Act
        var transaction = await _repository.GetTransactionByIdAsync(existingTransactionId);

        // Assert
        Assert.NotNull(transaction);
        Assert.IsType<Transaction>(transaction);
    }

    [Fact]
    public async Task GetUniqueTransactionAsync_ShouldSucceed()
    {
        // Arrange
        var existingTransaction = (await _repository.GetAllTransactionsAsync()).ToArray()[2];

        // Act
        var transaction = await _repository.GetUniqueTransactionAsync(existingTransaction);

        // Assert
        Assert.NotNull(transaction);
        Assert.IsType<Transaction>(transaction);
    }

    [Fact]
    public async Task DoesTransactionExistByIdAsync_ShouldSucceed()
    {
        // Arrange
        var existingTransaction = (await _repository.GetAllTransactionsAsync()).ToArray()[2];

        // Act
        var transaction = await _repository.DoesTransactionExistByIdAsync(existingTransaction);

        // Assert
        Assert.True(transaction);
    }

    [Fact]
    public async Task DoesTransactionExistByUniqueAsync_ShouldSucceed()
    {
        // Arrange
        var existingTransaction = (await _repository.GetAllTransactionsAsync()).ToArray()[2];

        // Act
        var transaction = await _repository.DoesTransactionExistByUniqueAsync(existingTransaction);

        // Assert
        Assert.True(transaction);
    }

    [Fact]
    public async Task AddTransactionAsync_ShouldSucceed()
    {
        // Arrange
        var newTransaction = new Transaction
        {
            TransactionId = default,
            TransactionType = "Deposit",
            OwnerName = "John Doe",
            BankName = "Bank of Tests"
        };

        // Act
        var result = await _repository.AddTransactionAsync(newTransaction);

        // Assert
        Assert.True(result);

        // Tear down
        await _connection.OpenAsync();
        await _connection.ExecuteAsync
            (
                """
                    DELETE FROM cash_flow.transaction
                    WHERE
                        transaction_type = @TransactionType
                        AND owner_name = @OwnerName
                        AND bank_name = @BankName;
                """,
                new
                {
                    TransactionType = newTransaction.TransactionType,
                    OwnerName = newTransaction.OwnerName,
                    BankName = newTransaction.BankName
                }
            );
        await _connection.CloseAsync();
    }

    [Fact]
    public async Task UpdateTransactionAsync_ShouldSucceed()
    {
        // Arrange
        var newTransaction = new Transaction
        {
            TransactionId = default,
            TransactionType = "Deposit",
            OwnerName = "John Doe",
            BankName = "Bank of Tests"
        };

        await _repository.AddTransactionAsync(newTransaction);
        var transactionId = (await _repository.GetUniqueTransactionAsync(newTransaction))?.TransactionId;

        var toUpdateTransaction = new Transaction
        {
            TransactionId = transactionId,
            TransactionType = "Updated",
            OwnerName = "Updated",
            BankName = "Updated"
        };

        // Act
        var result = await _repository.UpdateTransactionAsync(toUpdateTransaction);
        var updatedTransaction = await _repository.GetTransactionByIdAsync(transactionId);

        // Assert
        Assert.True(result);
        Assert.Equal(toUpdateTransaction.TransactionId, updatedTransaction?.TransactionId);
        Assert.Equal(toUpdateTransaction.TransactionType, updatedTransaction?.TransactionType);
        Assert.Equal(toUpdateTransaction.OwnerName, updatedTransaction?.OwnerName);
        Assert.Equal(toUpdateTransaction.BankName, updatedTransaction?.BankName);

        // Tear down
        await _connection.OpenAsync();
        await _connection.ExecuteAsync
            (
                """
                    DELETE FROM cash_flow.transaction
                    WHERE
                        transaction_type = @TransactionType
                        AND owner_name = @OwnerName
                        AND bank_name = @BankName;
                """,
                new
                {
                    TransactionType = newTransaction.TransactionType,
                    OwnerName = newTransaction.OwnerName,
                    BankName = newTransaction.BankName
                }
            );
        await _connection.CloseAsync();
    }
}
