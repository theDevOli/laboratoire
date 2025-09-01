using System.Data;
using Dapper;

using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;

namespace Laboratoire.Infrastructure.Repository;

public sealed class HazardRepository(DataContext dapper) : IHazardRepository
{
    #region SQL queries
    private readonly string _getAllHazardSql =
    $"""
    SELECT 
        hazard_id AS {nameof(Hazard.HazardId)},
        hazard_class AS {nameof(Hazard.HazardClass)},
        hazard_name AS {nameof(Hazard.HazardName)}
    FROM
        inventory.hazard;
    """;
    private readonly string _getHazardByIdSql =
    $"""
    SELECT 
        hazard_id AS {nameof(Hazard.HazardId)},
        hazard_class AS {nameof(Hazard.HazardClass)},
        hazard_name AS {nameof(Hazard.HazardName)}
    FROM 
        inventory.hazard
    WHERE 
        hazard_id = @HazardIdParameter;
    """;
    private readonly string _getHazardByClassSql =
    $"""
    SELECT 
        hazard_id AS {nameof(Hazard.HazardId)},
        hazard_class AS {nameof(Hazard.HazardClass)},
        hazard_name AS {nameof(Hazard.HazardName)}
    FROM 
        inventory.hazard
    WHERE 
        hazard_class = @HazardClassParameter;
    """;
    private readonly string _addHazardSql =
    $"""
    INSERT INTO inventory.hazard(
        hazard_class,
        hazard_name
    )
    VALUES
        (@HazardClassParameter,@HazardNameParameter);
    """;
    private readonly string _updateHazardSql =
    $"""
    UPDATE inventory.hazard
    SET
        hazard_class = @HazardClassParameter,
        hazard_name = @HazardNameParameter
    WHERE
        hazard_id = @HazardIdParameter;
    """;
    #endregion
    public async Task<bool> AddHazardAsync(Hazard hazard)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@HazardClassParameter", hazard.HazardClass, DbType.String);
        parameters.Add("@HazardNameParameter", hazard.HazardName, DbType.String);

        return await dapper.ExecuteSqlAsync(_addHazardSql, parameters);
    }
    public async Task<bool> DoesHazardExistByIdAsync(Hazard hazard)
    => await GetHazardByIdAsync(hazard.HazardId) is not null;
    public async Task<bool> DoesHazardExistByClassAsync(Hazard hazard)
    => await GetHazardByClassAsync(hazard.HazardClass) is not null;
    public async Task<IEnumerable<Hazard>> GetAllHazardsAsync()
    => await dapper.LoadDataAsync<Hazard>(_getAllHazardSql);
    public async Task<Hazard?> GetHazardByClassAsync(string? hazardClass)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@HazardClassParameter", hazardClass, DbType.String);

        return await dapper.LoadDataSingleAsync<Hazard>(_getHazardByClassSql, parameters);
    }
    public async Task<Hazard?> GetHazardByIdAsync(int? hazardId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@HazardIdParameter", hazardId, DbType.Int32);

        return await dapper.LoadDataSingleAsync<Hazard>(_getHazardByIdSql, parameters);
    }
    public async Task<bool> UpdateHazardAsync(Hazard hazard)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@HazardClassParameter", hazard.HazardClass, DbType.String);
        parameters.Add("@HazardNameParameter", hazard.HazardName, DbType.String);
        parameters.Add("@HazardIdParameter", hazard.HazardId, DbType.Int32);

        return await dapper.ExecuteSqlAsync(_updateHazardSql, parameters);
    }
}
