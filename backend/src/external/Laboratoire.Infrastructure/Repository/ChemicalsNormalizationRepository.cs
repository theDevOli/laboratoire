using System.Data;
using Dapper;

using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;

namespace Laboratoire.Infrastructure.Repository;

public sealed class ChemicalsNormalizationRepository(DataContext dapper) : IChemicalsNormalizationRepository
{
    #region SQL queries
    private readonly string _getAllChemicalsSql =
    $"""
    SELECT
        chemical_id AS {nameof(ChemicalsNormalization.ChemicalId)},
        hazard_id AS {nameof(ChemicalsNormalization.HazardId)}
    FROM 
        inventory.chemical_hazard;
    """;
    private readonly string _getChemicalsByIdSql =
    $"""
    SELECT
        chemical_id AS {nameof(ChemicalsNormalization.ChemicalId)},
        hazard_id AS {nameof(ChemicalsNormalization.HazardId)}
    FROM 
        inventory.chemical_hazard
    WHERE 
        chemical_id = @ChemicalIdParameter;
    """;
    private readonly string _deleteChemicalsSql =
    $"""
    DELETE FROM 
        inventory.chemical_hazard
    WHERE 
        chemical_id = @ChemicalIdParameter;
    """;
    private readonly string _countChemicalsSql =
    $"""
    SELECT
        COUNT(chemical_id)
    FROM 
        inventory.chemical_hazard
    WHERE 
        chemical_id = @ChemicalIdParameter;
    """;
    #endregion
    public async Task<bool> AddHazardAsync(IEnumerable<ChemicalsNormalization> chemicalsNormalizations)
    {
        var (sql, parameters) = Utils.Utils.BulkSqlStatement(chemicalsNormalizations, "chemical_hazard", "inventory");
        return await dapper.ExecuteSqlAsync(sql, parameters);
    }
    public async Task<int?> CountHazardAsync(int? chemicalId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ChemicalIdParameter", chemicalId, DbType.Int32);

        return await dapper.LoadDataSingleAsync<int>(_countChemicalsSql, parameters);
    }
    public async Task<bool> DeleteHazardAsync(int? chemicalId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ChemicalIdParameter", chemicalId, DbType.Int32);

        return await dapper.ExecuteSqlAsync(_deleteChemicalsSql, parameters);
    }
    public async Task<IEnumerable<ChemicalsNormalization>> GetAllHazardsAsync()
    => await dapper.LoadDataAsync<ChemicalsNormalization>(_getAllChemicalsSql);
    public async Task<IEnumerable<ChemicalsNormalization>?> GetHazardsByIdAsync(int? chemicalId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ChemicalIdParameter", chemicalId, DbType.Int32);

        return await dapper.LoadDataAsync<ChemicalsNormalization>(_getChemicalsByIdSql, parameters);
    }
}
