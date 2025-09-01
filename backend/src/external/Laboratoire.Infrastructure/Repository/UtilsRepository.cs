using System.Data;
using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Infrastructure.DbContext;

namespace Laboratoire.Infrastructure.Repository;

public sealed class UtilsRepository(DataContext dapper): IUtilsRepository
{
    #region SQL queries
    private readonly string _getAllStatesSql =
    $"""
    SELECT
        state_id AS {nameof(State.StateId)},
        state_name AS {nameof(State.StateName)},
        state_code AS {nameof(State.StateCode)}
    FROM
        utils.state
    """;
    private readonly string _getPostalCodeSql =
    $"""
    SELECT 
        p.postal_code AS {nameof(Property.PostalCode)}
    FROM
        customers.property AS p 
    WHERE
        p.city=@CityParameter
        AND p.state_id=@StateParameter;
    """;
    #endregion
    public async Task<IEnumerable<State>?> GetAllStatesAsync()
    => await dapper.LoadDataAsync<State>(_getAllStatesSql);
    public async Task<string?> GetPostalCodeByCityAndStateAsync(string? city, int? stateId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@CityParameter", city, DbType.String);
        parameters.Add("@StateParameter", stateId, DbType.Int32);

        var test = await dapper.LoadDataSingleAsync<string>(_getPostalCodeSql, parameters);
        return test;
    }
}
