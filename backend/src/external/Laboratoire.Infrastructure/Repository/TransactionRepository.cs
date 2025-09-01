using System.Data;
using Dapper;

using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;

namespace Laboratoire.Infrastructure.Repository;

public sealed class TransactionRepository(DataContext dapper) : ITransactionRepository
{
    #region SQL queries
    private readonly string _getAllTransactionsSql =
    $"""
    SELECT
        transaction_id AS {nameof(Transaction.TransactionId)},
        transaction_type AS {nameof(Transaction.TransactionType)},
        owner_name AS {nameof(Transaction.OwnerName)},
        bank_name AS {nameof(Transaction.BankName)}
    FROM
        cash_flow.transaction
    """;
    private readonly string _getTransactionByIdSql =
    $"""
    SELECT
        transaction_id AS {nameof(Transaction.TransactionId)},
        transaction_type AS {nameof(Transaction.TransactionType)},
        owner_name AS {nameof(Transaction.OwnerName)},
        bank_name AS {nameof(Transaction.BankName)}
    FROM
        cash_flow.transaction
    WHERE
        transaction_id = @TransactionIdParameter
    """;
    private readonly string _getUniqueTransactionSql =
    $"""
    SELECT
        transaction_id AS {nameof(Transaction.TransactionId)},
        transaction_type AS {nameof(Transaction.TransactionType)},
        owner_name AS {nameof(Transaction.OwnerName)},
        bank_name AS {nameof(Transaction.BankName)}
    FROM
        cash_flow.transaction
    WHERE
        transaction_type = @TransactionTypeParameter
        AND owner_name = @OwnerNameParameter
        AND bank_name = @BankNameParameter;
    """;
    private readonly string _addTransactionSql =
    $"""
    INSERT INTO cash_flow.transaction(
        transaction_type,
        owner_name,
        bank_name
    )
    VALUES
    (
        @TransactionTypeParameter,
        @OwnerNameParameter,
        @BankNameParameter
    );
    """;
    private readonly string _updateTransactionSql =
    $"""
    UPDATE cash_flow.transaction
    SET
        transaction_type = @TransactionTypeParameter,
        owner_name = @OwnerNameParameter,
        bank_name = @BankNameParameter
    WHERE
        transaction_id = @TransactionIdParameter;
    """;
    #endregion
    public async Task<bool> AddTransactionAsync(Transaction transaction)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@TransactionTypeParameter", transaction.TransactionType, DbType.String);
        parameters.Add("@OwnerNameParameter", transaction.OwnerName, DbType.String);
        parameters.Add("@BankNameParameter", transaction.BankName, DbType.String);

        return await dapper.ExecuteSqlAsync(_addTransactionSql, parameters);
    }
    public async Task<bool> DoesTransactionExistByIdAsync(Transaction transaction)
    => await GetTransactionByIdAsync(transaction.TransactionId) is not null;
    public async Task<bool> DoesTransactionExistByUniqueAsync(Transaction transaction)
    => await GetUniqueTransactionAsync(transaction) is not null;
    public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
    => await dapper.LoadDataAsync<Transaction>(_getAllTransactionsSql);
    public async Task<Transaction?> GetTransactionByIdAsync(int? transactionId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@TransactionIdParameter", transactionId, DbType.Int32);

        return await dapper.LoadDataSingleAsync<Transaction>(_getTransactionByIdSql, parameters);
    }
    public async Task<Transaction?> GetUniqueTransactionAsync(Transaction transaction)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@TransactionTypeParameter", transaction.TransactionType, DbType.String);
        parameters.Add("@OwnerNameParameter", transaction.OwnerName, DbType.String);
        parameters.Add("@BankNameParameter", transaction.BankName, DbType.String);

        return await dapper.LoadDataSingleAsync<Transaction>(_getUniqueTransactionSql, parameters);
    }
    public async Task<bool> UpdateTransactionAsync(Transaction transaction)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@TransactionTypeParameter", transaction.TransactionType, DbType.String);
        parameters.Add("@OwnerNameParameter", transaction.OwnerName, DbType.String);
        parameters.Add("@BankNameParameter", transaction.BankName, DbType.String);
        parameters.Add("@TransactionIdParameter", transaction.TransactionId, DbType.Int32);

        return await dapper.ExecuteSqlAsync(_updateTransactionSql, parameters);
    }
}
