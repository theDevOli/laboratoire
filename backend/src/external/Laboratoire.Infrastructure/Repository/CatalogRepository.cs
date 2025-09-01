using System.Data;
using Dapper;

using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;

namespace Laboratoire.Infrastructure.Repository;

public sealed class CatalogRepository(DataContext dapper): ICatalogRepository
{
    #region SQL queries
    private readonly string _getAllCatalogsSql =
    $"""
    SELECT
        catalog_id AS {nameof(Catalog.CatalogId)},
        report_type AS {nameof(Catalog.ReportType)},
        sample_type AS {nameof(Catalog.SampleType)},
        label_name AS {nameof(Catalog.LabelName)},
        legends AS {nameof(Catalog.Legends)},
        price AS {nameof(Catalog.Price)}
    FROM 
        parameters.catalog;
    """;
    private readonly string _getCatalogByIdSql =
    $"""
    SELECT
        catalog_id AS {nameof(Catalog.CatalogId)},
        report_type AS {nameof(Catalog.ReportType)},
        sample_type AS {nameof(Catalog.SampleType)},
        label_name AS {nameof(Catalog.LabelName)},
        legends AS {nameof(Catalog.Legends)},
        price AS {nameof(Catalog.Price)}
    FROM 
        parameters.catalog
    WHERE 
        catalog_id = @CatalogIdParameter;
    """;
    private readonly string _getUniqueCatalogSql =
    $"""
    SELECT
        catalog_id AS {nameof(Catalog.CatalogId)},
        report_type AS {nameof(Catalog.ReportType)},
        sample_type AS {nameof(Catalog.SampleType)},
        label_name AS {nameof(Catalog.LabelName)},
        legends AS {nameof(Catalog.Legends)},
        price AS {nameof(Catalog.Price)}
    FROM 
        parameters.catalog
    WHERE 
        report_type = @ReportTypeParameter
        AND sample_type = @SampleTypeParameter;
    """;
    private readonly string _addCatalogSql =
    $"""
    INSERT INTO parameters.catalog(
        report_type,
        sample_type,
        label_name,
        legends,
        price
    )
    VALUES(
        @ReportTypeParameter,
        @SampleTypeParameter,
        @LabelNameParameter,
        @LegendsParameter::JSONB[],
        @PriceParameter
    );
    """;
    private readonly string _updateCatalogSql =
    $"""
    UPDATE parameters.catalog
     SET
        report_type = @ReportTypeParameter,
        sample_type = @SampleTypeParameter,
        label_name = @LabelNameParameter,
        legends = @LegendsParameter::JSONB[],
        price = @PriceParameter
    WHERE 
        catalog_id = @CatalogIdParameter;
    """;
    #endregion
    public async Task<bool> AddCatalogAsync(Catalog catalog)
    {
        var catalogDto = catalog.ToDb();
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ReportTypeParameter", catalogDto.ReportType, DbType.String);
        parameters.Add("@SampleTypeParameter", catalogDto.SampleType, DbType.String);
        parameters.Add("@LabelNameParameter", catalogDto.LabelName, DbType.String);
        parameters.Add("@LegendsParameter", catalogDto.Legends, DbType.Object);
        parameters.Add("@PriceParameter", catalogDto.Price, DbType.Decimal);

        return await dapper.ExecuteSqlAsync(_addCatalogSql, parameters);
    }

    public async Task<bool> DoesCatalogExistByIdAsync(Catalog catalog)
    => await GetCatalogByIdAsync(catalog.CatalogId) is not null;
    public async Task<bool> DoesCatalogExistByUniqueAsync(Catalog catalog)
    => await GetUniqueCatalogAsync(catalog) is not null;


    public async Task<IEnumerable<Catalog>> GetAllCatalogsAsync()
    {
        var catalogDto = await dapper.LoadDataAsync<CatalogDtoDb>(_getAllCatalogsSql);

        return catalogDto.Select(catalog => catalog.FromDb());
    }

    public async Task<Catalog?> GetCatalogByIdAsync(int? catalogId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@CatalogIdParameter", catalogId, DbType.Int32);

        var catalogDto = await dapper.LoadDataSingleAsync<CatalogDtoDb>(_getCatalogByIdSql, parameters);

        return catalogDto?.FromDb();
    }

    public async Task<Catalog?> GetUniqueCatalogAsync(Catalog catalog)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ReportTypeParameter", catalog.ReportType, DbType.String);
        parameters.Add("@SampleTypeParameter", catalog.SampleType, DbType.String);

        var catalogDto = await dapper.LoadDataSingleAsync<CatalogDtoDb>(_getUniqueCatalogSql, parameters);
        return catalogDto?.FromDb();
    }

    public async Task<bool> UpdateCatalogAsync(Catalog catalog)
    {
        var catalogDto = catalog.ToDb();
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ReportTypeParameter", catalogDto.ReportType, DbType.String);
        parameters.Add("@SampleTypeParameter", catalogDto.SampleType, DbType.String);
        parameters.Add("@LabelNameParameter", catalogDto.LabelName, DbType.String);
        parameters.Add("@LegendsParameter", catalogDto.Legends, DbType.Object);
        parameters.Add("@PriceParameter", catalogDto.Price, DbType.Decimal);
        parameters.Add("@CatalogIdParameter", catalogDto.CatalogId, DbType.Int32);

        return await dapper.ExecuteSqlAsync(_updateCatalogSql, parameters);
    }
}
