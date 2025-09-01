using System.Data;
using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Infrastructure.DbContext;

namespace Laboratoire.Infrastructure.Repository;

public sealed class CashFlowRepository(DataContext dapper): ICashFlowRepository
{
    #region SQL queries
    private readonly string _getCashFlowByYearAndMonth =
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
        payment_date IS NOT NULL
        AND EXTRACT(YEAR FROM payment_date) = @YearParameter
        AND EXTRACT(MONTH FROM payment_date) = @MonthParameter
    ORDER BY 
        payment_date, description;
    """;
    private readonly string _getAmountByYearAndMonth =
    """
    SELECT 
        COALESCE(SUM(total_paid), 0)
    FROM 
        cash_flow.cash_flow
    WHERE 
        payment_date IS NOT NULL
        AND EXTRACT(YEAR FROM payment_date) = @YearParameter
        AND EXTRACT(MONTH FROM payment_date) = @MonthParameter
        AND (@TransactionFilter IS NULL 
        OR (transaction_id = @TransactionFilter AND transaction_id > 1))
        AND (
            @CashFlowFilter = 'all' OR
            (@CashFlowFilter = 'in' AND total_paid > 0) OR
            (@CashFlowFilter = 'out' AND total_paid < 0)
        );
    """;
    private readonly string _getAllCashFlowSql =
    $"""
    SELECT 
        cash_flow_id AS {nameof(CashFlow.CashFlowId)},
        transaction_id AS {nameof(CashFlow.TransactionId)},
        description AS {nameof(CashFlow.Description)},
        partner_id AS {nameof(CashFlow.PartnerId)},
        total_paid AS {nameof(CashFlow.TotalPaid)},
        payment_date AS {nameof(CashFlow.PaymentDate)}
    FROM 
        cash_flow.cash_flow;
    """;
    private readonly string _getCashFlowByIdSql =
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
        cash_flow_id = @CashFlowIdParameter;
    """;
    private readonly string _updateFlowBySql =
    """
    UPDATE cash_flow.cash_flow
    SET
        transaction_id = @TransactionIdParameter,
        description = @DescriptionParameter,
        partner_id = @PartnerIdParameter,
        total_paid = @TotalPaidParameter,
        payment_date = @PaymentDateParameter
    WHERE 
        cash_flow_id = @CashFlowIdParameter;
    """;
    private readonly string _patchDescriptionSql =
    """
    UPDATE cash_flow.cash_flow
    SET
        description = @DescriptionParameter
    WHERE 
        cash_flow_id = @CashFlowIdParameter;
    """;
    private readonly string _addFlowBySql =
    """
    INSERT INTO cash_flow.cash_flow (
        transaction_id,
        description,
        partner_id,
        total_paid,
        payment_date
    )
    VALUES (
        @TransactionIdParameter,
        @DescriptionParameter,
        @PartnerIdParameter,
        @TotalPaidParameter,
        @PaymentDateParameter
    )
    RETURNING cash_Flow_id;
    """;
    #endregion
    public async Task<int> AddCashFlowAsync(CashFlow cashFlow)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@DescriptionParameter", cashFlow.Description, DbType.String);
        parameters.Add("@TransactionIdParameter", cashFlow.TransactionId, DbType.Int32);
        parameters.Add("@PartnerIdParameter", cashFlow.PartnerId, DbType.Guid);
        parameters.Add("@TotalPaidParameter", cashFlow.TotalPaid, DbType.Decimal);
        parameters.Add("@PaymentDateParameter", cashFlow.PaymentDate, DbType.Date);

        return await dapper.LoadDataSingleAsync<int>(_addFlowBySql, parameters);
    }

    public async Task<bool> DoesCashFlowExistsAsync(CashFlow cashFlow)
    => await GetCashFlowByIdAsync(cashFlow.CashFlowId) is not null;

    public async Task<IEnumerable<CashFlow>> GetAllCashFlowAsync()
    => await dapper.LoadDataAsync<CashFlow>(_getAllCashFlowSql);

    public async Task<IEnumerable<CashFlow>> GetCashFlowByYearAndMonthAsync(int? year, int? month)
    {
        DynamicParameters parameter = new DynamicParameters();
        parameter.Add("@YearParameter", year, DbType.Int32);
        parameter.Add("@MonthParameter", month, DbType.Int32);

        return await dapper.LoadDataAsync<CashFlow>(_getCashFlowByYearAndMonth, parameter);
    }

    public async Task<decimal?> GetAmountAsync(int? year, int? month, string? cashFlow, int? transaction)
    {
        DynamicParameters parameter = new DynamicParameters();
        parameter.Add("@YearParameter", year, DbType.Int32);
        parameter.Add("@MonthParameter", month, DbType.Int32);
        parameter.Add("@CashFlowFilter", cashFlow, DbType.String);
        parameter.Add("@TransactionFilter", transaction, DbType.Int32);

        return await dapper.LoadDataSingleAsync<decimal>(_getAmountByYearAndMonth, parameter);
    }

    public async Task<CashFlow?> GetCashFlowByIdAsync(int? cashFlowId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@CashFlowIdParameter", cashFlowId, DbType.Int32);

        return await dapper.LoadDataSingleAsync<CashFlow>(_getCashFlowByIdSql, parameters);
    }

    public async Task<bool> UpdateCashFlowAsync(CashFlow cashFlow)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@DescriptionParameter", cashFlow.Description, DbType.String);
        parameters.Add("@TransactionIdParameter", cashFlow.TransactionId, DbType.Int32);
        parameters.Add("@PartnerIdParameter", cashFlow.PartnerId, DbType.Guid);
        parameters.Add("@TotalPaidParameter", cashFlow.TotalPaid, DbType.Decimal);
        parameters.Add("@PaymentDateParameter", cashFlow.PaymentDate, DbType.Date);
        parameters.Add("@CashFlowIdParameter", cashFlow.CashFlowId, DbType.Int32);

        return await dapper.ExecuteSqlAsync(_updateFlowBySql, parameters);
    }

    public async Task<bool> PatchDescriptionAsync(CashFlow cashFlow)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@DescriptionParameter", cashFlow.Description, DbType.String);
        parameters.Add("@CashFlowIdParameter", cashFlow.CashFlowId, DbType.Int32);

        return await dapper.ExecuteSqlAsync(_patchDescriptionSql, parameters);
    }
}
