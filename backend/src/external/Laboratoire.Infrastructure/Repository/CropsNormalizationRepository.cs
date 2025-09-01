using System.Data;
using Dapper;

using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;

namespace Laboratoire.Infrastructure.Repository;

public sealed class CropsNormalizationRepository(DataContext dapper) : ICropsNormalizationRepository
{
#region SQL queries
    private readonly string _deleteCropSql =
    $"""
    DELETE FROM 
        document.crop_protocol
    WHERE 
        protocol_id = @ProtocolIdParameter;
    """;
    private readonly string _getAllCropsSql =
    $"""
    SELECT
        crop_id AS {nameof(CropsNormalization.CropId)},
        protocol_id AS {nameof(CropsNormalization.ProtocolId)}
    FROM 
        document.crop_protocol;
    """;
    private readonly string _getCropsByReportIdSql =
    $"""
    SELECT
        cp.crop_id AS {nameof(CropsNormalization.CropId)},
        cp.protocol_id AS {nameof(CropsNormalization.ProtocolId)}
    FROM 
        document.crop_protocol AS cp
    INNER JOIN 
        document.protocol AS dp
        ON cp.protocol_id = dp.protocol_id
    WHERE 
        report_id = @ReportIdParameter;
    """;
    private readonly string _countCropSql =
    $"""
    SELECT
            COUNT(protocol_id)
        FROM 
            document.crop_protocol
        WHERE 
            protocol_id = @ProtocolIdParameter;
    """;
    #endregion
    public async Task<bool> AddCropsAsync(IEnumerable<CropsNormalization> cropsNormalization)
    {
        var (sql, parameters) = Utils.Utils.BulkSqlStatement(cropsNormalization, "crop_protocol", "document");

        return await dapper.ExecuteSqlAsync(sql, parameters);
    }
    public async Task<bool> IsThereNoneCropsAsync(string? protocolId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ProtocolIdParameter", protocolId, DbType.String);

        return await dapper.LoadDataSingleAsync<int>(_countCropSql, parameters) == 0;
    }
    public async Task<bool> DeleteCropsAsync(string? protocolId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ProtocolIdParameter", protocolId, DbType.String);

        return await dapper.ExecuteSqlAsync(_deleteCropSql, parameters);
    }
    public async Task<IEnumerable<CropsNormalization>> GetAllCropsAsync()
    => await dapper.LoadDataAsync<CropsNormalization>(_getAllCropsSql);
    public async Task<IEnumerable<CropsNormalization>?> GetCropByReportIdAsync(Guid? reportId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ReportIdParameter", reportId, DbType.Guid);

        return await dapper.LoadDataAsync<CropsNormalization>(_getCropsByReportIdSql, parameters);
    }
}
