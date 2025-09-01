using System.Data;
using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Infrastructure.DbContext;

namespace Laboratoire.Infrastructure.Repository;

public sealed class FertilizerRepository(DataContext dapper) : IFertilizerRepository
{
    #region SQL queries
    private readonly string _getAllFertilizersSql =
    $"""
    SELECT
        fertilizer_id AS {nameof(Fertilizer.FertilizerId)},
        nitrogen AS {nameof(Fertilizer.Nitrogen)},
        phosphorus AS {nameof(Fertilizer.Phosphorus)},
        potassium AS {nameof(Fertilizer.Potassium)},
        is_available AS {nameof(Fertilizer.IsAvailable)},
        proportion AS {nameof(Fertilizer.Proportion)}
    FROM
        document.fertilizer;
    """;
    private readonly string _getFertilizerByIdSql =
    $"""
    SELECT
        fertilizer_id AS {nameof(Fertilizer.FertilizerId)},
        nitrogen AS {nameof(Fertilizer.Nitrogen)},
        phosphorus AS {nameof(Fertilizer.Phosphorus)},
        potassium AS {nameof(Fertilizer.Potassium)},
        is_available AS {nameof(Fertilizer.IsAvailable)},
        proportion AS {nameof(Fertilizer.Proportion)}
    FROM 
        document.fertilizer
    WHERE 
        fertilizer_id = @FertilizerIdParameter;
    """;
    private readonly string _getFertilizerByProportionSql =
    $"""
    SELECT
        fertilizer_id AS {nameof(Fertilizer.FertilizerId)},
        nitrogen AS {nameof(Fertilizer.Nitrogen)},
        phosphorus AS {nameof(Fertilizer.Phosphorus)},
        potassium AS {nameof(Fertilizer.Potassium)},
        is_available AS {nameof(Fertilizer.IsAvailable)},
        proportion AS {nameof(Fertilizer.Proportion)}
    FROM 
        document.fertilizer
    WHERE 
        proportion = @ProportionParameter;
    """;
    private readonly string _changeFertilizerStatusSql =
    $"""
    UPDATE document.fertilizer
        SET
            proportion = @ProportionParameter
        WHERE 
            fertilizer_id = @FertilizerIdParameter;
    """;
    #endregion
    public async Task<bool> ChangeFertilizerStatusAsync(int? fertilizerId)
    {
        if (fertilizerId is null) return false;

        var fertilizer = await GetFertilizerByIdAsync(fertilizerId);
        if (fertilizer is null) return false;

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ProportionParameter", fertilizer.Proportion, DbType.String);
        parameters.Add("@FertilizerIdParameter", fertilizerId, DbType.Int32);

        return await dapper.ExecuteSqlAsync(_changeFertilizerStatusSql, parameters);
    }
    public async Task<IEnumerable<Fertilizer>> GetAllFertilizersAsync()
    => await dapper.LoadDataAsync<Fertilizer>(_getAllFertilizersSql);
    public async Task<Fertilizer?> GetFertilizerByIdAsync(int? fertilizerId)
    {
        if (fertilizerId is null) return null;

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@FertilizerIdParameter", fertilizerId, DbType.Int32);

        return await dapper.LoadDataSingleAsync<Fertilizer>(_getFertilizerByIdSql, parameters);
    }
    public async Task<IEnumerable<Fertilizer>> GetFertilizersByProportionAsync(string? proportion)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ProportionParameter", proportion, DbType.String);

        return await dapper.LoadDataAsync<Fertilizer>(_getFertilizerByProportionSql, parameters);
    }
}
