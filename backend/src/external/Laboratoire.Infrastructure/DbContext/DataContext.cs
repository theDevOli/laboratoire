using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Laboratoire.Infrastructure.DbContext;

public class DataContext
{

    private readonly IConfiguration? _config;


    private IDbConnection CreateConnection()
    {
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
        string? connectionString = environment == "Development"
    ? _config?.GetConnectionString("DefaultConnectionDev")
    : _config?.GetConnectionString("DefaultConnectionProd");
        return new NpgsqlConnection(connectionString);
    }


    public DataContext() { }
    public DataContext(IConfiguration config)
    {
        _config = config;
    }

    public virtual async Task<IEnumerable<T>> LoadDataAsync<T>(string sql, DynamicParameters? parameters = null)
    {
        using var dbConnection = CreateConnection();
        dbConnection.Open();
        return await dbConnection.QueryAsync<T>(sql, parameters);
    }

    public virtual async Task<T?> LoadDataSingleAsync<T>(string sql, DynamicParameters? parameters = null)
    {
        using var dbConnection = CreateConnection();
        dbConnection.Open();

        return await dbConnection.QueryFirstOrDefaultAsync<T>(sql, parameters);
    }

    public virtual async Task<bool> ExecuteSqlAsync(string sql, DynamicParameters? parameters = null)
    {
        using var dbConnection = CreateConnection();
        dbConnection.Open();

        return await dbConnection.ExecuteAsync(sql, parameters) > 0;
    }

    public virtual async Task<T?> ExecuteScalarSqlAsync<T>(string sql, DynamicParameters? parameters = null)
    {
        using var dbConnection = CreateConnection();
        dbConnection.Open();

        return await dbConnection.ExecuteScalarAsync<T>(sql, parameters);
    }

    public virtual async Task<int> ExecuteSqlWithRowCountAsync(string sql, DynamicParameters? parameters = null)
    {
        using var dbConnection = CreateConnection();
        dbConnection.Open();

        return await dbConnection.ExecuteAsync(sql, parameters);
    }
}
