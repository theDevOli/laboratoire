using System.Data;
using Dapper;

using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;

namespace Laboratoire.Infrastructure.Repository;

public sealed class ClientRepository(DataContext dapper) : IClientRepository
{
    #region SQL queries
    private readonly string _getAllClientSql =
    $"""
    SELECT 
        client_id AS {nameof(Client.ClientId)},
        client_name AS {nameof(Client.ClientName)},
        client_tax_id AS {nameof(Client.ClientTaxId)},
        client_email AS {nameof(Client.ClientEmail)},
        client_phone AS {nameof(Client.ClientPhone)}
    FROM 
        customers.client
    """;
    private readonly string _getClientByIdSql =
    $"""
    SELECT 
        client_id AS {nameof(Client.ClientId)},
        client_name AS {nameof(Client.ClientName)},
        client_tax_id AS {nameof(Client.ClientTaxId)},
        client_email AS {nameof(Client.ClientEmail)},
        client_phone AS {nameof(Client.ClientPhone)}
    FROM 
        customers.client
    WHERE 
        client_id = @ClientIdParameter;
    """;
    private readonly string _getClientByLikeTaxIdSql =
    $"""
    SELECT 
        client_id AS {nameof(Client.ClientId)},
        client_name AS {nameof(Client.ClientName)},
        client_tax_id AS {nameof(Client.ClientTaxId)},
        client_email AS {nameof(Client.ClientEmail)},
        client_phone AS {nameof(Client.ClientPhone)}
    FROM 
        customers.client
    WHERE 
        client_tax_id LIKE @Parameter
    ORDER BY
        client_tax_id;
    """;
    private readonly string _getClientByTaxIdSql =
    $"""
    SELECT 
        client_id AS {nameof(Client.ClientId)},
        client_name AS {nameof(Client.ClientName)},
        client_tax_id AS {nameof(Client.ClientTaxId)},
        client_email AS {nameof(Client.ClientEmail)},
        client_phone AS {nameof(Client.ClientPhone)}
    FROM 
        customers.client
    WHERE 
        client_tax_id =  @ClientTaxIdParameter;
    """;
    private readonly string _addSql =
    $"""
    INSERT INTO customers.client (
        client_name,
        user_id,
        client_tax_id,
        client_email,
        client_phone
    )
    VALUES 
    (
        @ClientNameParameter,
        @UserIdParameter,
        @ClientTaxIdParameter,
        @ClientEmailParameter,
        @ClientPhoneParameter
    );
    """;
    private readonly string _updateSql =
    $"""
    UPDATE customers.client 
    SET
        client_name = @ClientNameParameter,
        client_tax_id = @ClientTaxIdParameter,
        client_email = @ClientEmailParameter,
        client_phone = @ClientPhoneParameter
    WHERE   
        client_id = @ClientIdParameter;
    """;
    #endregion
    public async Task<bool> AddClientAsync(Client client, Guid? userId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ClientNameParameter", client.ClientName, DbType.String);
        parameters.Add("@UserIdParameter", userId, DbType.Guid);
        parameters.Add("@ClientTaxIdParameter", client.ClientTaxId, DbType.String);
        parameters.Add("@ClientEmailParameter", client.ClientEmail, DbType.String);
        parameters.Add("@ClientPhoneParameter", client.ClientPhone, DbType.String);

        return await dapper.ExecuteSqlAsync(_addSql, parameters);
    }
    public async Task<bool> DoesClientExistByTaxIdAsync(Client client)
    => await GetByTaxIdAsync(client.ClientTaxId) is not null;
    public async Task<bool> DoesClientExistByClientIdAsync(Client client)
    => await GetByClientIdAsync(client.ClientId) is not null;
    public async Task<IEnumerable<Client>> GetAllClientsAsync(string? filter)
    {
        var allowedFilters = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "ClientName",
            "ClientTaxId"
        };

        var sanitizedFilter = allowedFilters.Contains(filter ?? "")
            ? filter
            : "ClientName";

        string query = $"{_getAllClientSql} \nORDER BY {sanitizedFilter};";
        return await dapper.LoadDataAsync<Client>(query);
    }
    public async Task<Client?> GetByTaxIdAsync(string? taxId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ClientTaxIdParameter", taxId, DbType.String);

        return await dapper.LoadDataSingleAsync<Client?>(_getClientByTaxIdSql, parameters);
    }
    public async Task<Client?> GetByClientIdAsync(Guid? clientId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ClientIdParameter", clientId, DbType.Guid);

        return await dapper.LoadDataSingleAsync<Client?>(_getClientByIdSql, parameters);
    }
    public async Task<bool> UpdateClientAsync(Client client)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ClientNameParameter", client.ClientName, DbType.String);
        parameters.Add("@ClientTaxIdParameter", client.ClientTaxId, DbType.String);
        parameters.Add("@ClientEmailParameter", client.ClientEmail, DbType.String);
        parameters.Add("@ClientPhoneParameter", client.ClientPhone, DbType.String);
        parameters.Add("@ClientIdParameter", client.ClientId, DbType.Guid);

        return await dapper.ExecuteSqlAsync(_updateSql, parameters);
    }
    public async Task<IEnumerable<Client>> GetClientsLikeAsync(string? taxId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Parameter", $"%{taxId}%", DbType.String);

        return await dapper.LoadDataAsync<Client>(_getClientByLikeTaxIdSql, parameters);
    }
}
