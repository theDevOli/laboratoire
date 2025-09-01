using System.Data;
using Dapper;

using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;

namespace Laboratoire.Infrastructure.Repository;

public sealed class ParameterRepository(DataContext dapper) : IParameterRepository
{
    #region SQL queries
    private readonly string _getAllParametersSql =
    $"""
    SELECT
        parameter_id AS {nameof(Parameter.ParameterId)},
        catalog_id AS {nameof(Parameter.CatalogId)},
        parameter_name AS {nameof(Parameter.ParameterName)},
        input_quantity AS {nameof(Parameter.InputQuantity)},
        unit AS {nameof(Parameter.Unit)},
        official_doc AS {nameof(Parameter.OfficialDoc)},
        vmp AS {nameof(Parameter.Vmp)},
        equation AS {nameof(Parameter.Equation)}
    FROM
        parameters.parameter;
    """;
    private readonly string _getParameterByIdSql =
    $"""
    SELECT
        parameter_id AS {nameof(Parameter.ParameterId)},
        catalog_id AS {nameof(Parameter.CatalogId)},
        parameter_name AS {nameof(Parameter.ParameterName)},
        input_quantity AS {nameof(Parameter.InputQuantity)},
        unit AS {nameof(Parameter.Unit)},
        official_doc AS {nameof(Parameter.OfficialDoc)},
        vmp AS {nameof(Parameter.Vmp)},
        equation AS {nameof(Parameter.Equation)}
    FROM 
        parameters.parameter
    WHERE 
        parameter_id = @ParameterIdParameter;
    """;
    private readonly string _getParameterByReportIdSql =
    $"""
    SELECT
        pp.parameter_id AS {nameof(Parameter.ParameterId)},
        pp.catalog_id AS {nameof(Parameter.CatalogId)},
        pp.parameter_name AS {nameof(Parameter.ParameterName)},
        pp.unit AS {nameof(Parameter.Unit)},
        pp.input_quantity AS {nameof(Parameter.InputQuantity)},
        pp.official_doc AS {nameof(Parameter.OfficialDoc)},
        pp.vmp AS {nameof(Parameter.Vmp)},
        pp.equation AS {nameof(Parameter.Equation)}
    FROM 
        document.protocol AS dp
    INNER JOIN 
        parameters.parameter AS pp 
        ON dp.catalog_id = pp.catalog_id 
    WHERE 
        dp.report_id = @ReportIdParameter;
    """;
    private readonly string _getParameterInputByIdSql =
    $"""
    SELECT
        parameter_id AS {nameof(Parameter.ParameterId)},
        parameter_name AS {nameof(Parameter.ParameterName)},
        input_quantity AS {nameof(Parameter.InputQuantity)}
    FROM 
        parameters.parameter
    WHERE 
        catalog_id = @CatalogIdParameter;
    """;
    private readonly string _getUniqueParameterSql =
    $"""
    SELECT
        parameter_id AS {nameof(Parameter.ParameterId)},
        catalog_id AS {nameof(Parameter.CatalogId)},
        parameter_name AS {nameof(Parameter.ParameterName)},
        input_quantity AS {nameof(Parameter.InputQuantity)},
        unit AS {nameof(Parameter.Unit)},
        official_doc AS {nameof(Parameter.OfficialDoc)},
        vmp AS {nameof(Parameter.Vmp)},
        equation AS {nameof(Parameter.Equation)}
    FROM 
        parameters.parameter
    WHERE 
        catalog_id = @CatalogIdParameter
        AND parameter_name = @ParameterNameParameter;
    """;
    private readonly string _addParameterSql =
    $"""
    INSERT INTO parameters.parameter (
        catalog_id,
        parameter_name,
        unit,
        input_quantity,
        official_doc,
        vmp,
        equation
    )
    VALUES 
    (
        @CatalogIdParameter,
        @ParameterNameParameter, 
        @UnitParameter,
        @InputQuantityParameter,
        @OfficialDocParameter,
        @VmpParameter,
        @EquationParameter
    );
    """;
    private readonly string _updateParameterSql =
    $"""
    UPDATE parameters.parameter
    SET
        catalog_id = @CatalogIdParameter,
        parameter_name = @ParameterNameParameter, 
        unit = @UnitParameter,
        input_quantity = @InputQuantityParameter,
        official_doc = @OfficialDocParameter,
        vmp = @VmpParameter,
        equation = @EquationParameter
    WHERE 
        parameter_id = @ParameterIdParameter;
    """;
    #endregion
    public async Task<bool> AddParameterAsync(Parameter parameter)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@CatalogIdParameter", parameter.CatalogId, DbType.Int32);
        parameters.Add("@ParameterNameParameter", parameter.ParameterName, DbType.String);
        parameters.Add("@UnitParameter", parameter.Unit, DbType.String);
        parameters.Add("@InputQuantityParameter", parameter.InputQuantity, DbType.Int32);
        parameters.Add("@OfficialDocParameter", parameter.OfficialDoc, DbType.String);
        parameters.Add("@VmpParameter", parameter.Vmp, DbType.String);
        parameters.Add("@EquationParameter", parameter.Equation, DbType.String);

        return await dapper.ExecuteSqlAsync(_addParameterSql, parameters);
    }
    public async Task<bool> DoesParameterExistByParameterIdAsync(Parameter parameter)
    => await GetParameterByParameterIdAsync(parameter.ParameterId) is not null;
    public async Task<bool> IsParameterUniqueAsync(Parameter parameter)
    => await GetUniqueParameterAsync(parameter) is not null;
    public async Task<IEnumerable<Parameter>> GetAllParametersAsync()
    => await dapper.LoadDataAsync<Parameter>(_getAllParametersSql);
    public async Task<Parameter?> GetUniqueParameterAsync(Parameter parameter)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@CatalogIdParameter", parameter.CatalogId, DbType.Int32);
        parameters.Add("@ParameterNameParameter", parameter.ParameterName, DbType.String);

        return await dapper.LoadDataSingleAsync<Parameter>(_getUniqueParameterSql, parameters);
    }
    public async Task<Parameter?> GetParameterByParameterIdAsync(int? parameterId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ParameterIdParameter", parameterId, DbType.Int32);

        return await dapper.LoadDataSingleAsync<Parameter>(_getParameterByIdSql, parameters);
    }
    public async Task<IEnumerable<Parameter>?> GetParametersByReportIdAsync(Guid? reportId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ReportIdParameter", reportId, DbType.Guid);

        return await dapper.LoadDataAsync<Parameter>(_getParameterByReportIdSql, parameters);
    }
    public async Task<bool> UpdateParameterAsync(Parameter parameter)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ParameterIdParameter", parameter.ParameterId, DbType.Int32);
        parameters.Add("@CatalogIdParameter", parameter.CatalogId, DbType.Int32);
        parameters.Add("@ParameterNameParameter", parameter.ParameterName, DbType.String);
        parameters.Add("@UnitParameter", parameter.Unit, DbType.String);
        parameters.Add("@InputQuantityParameter", parameter.InputQuantity, DbType.Int32);
        parameters.Add("@OfficialDocParameter", parameter.OfficialDoc, DbType.String);
        parameters.Add("@VmpParameter", parameter.Vmp, DbType.String);
        parameters.Add("@EquationParameter", parameter.Equation, DbType.String);

        return await dapper.ExecuteSqlAsync(_updateParameterSql, parameters);
    }
    public async Task<IEnumerable<T>?> GetParametersInputByCatalogIdAsync<T>(int? catalogId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@CatalogIdParameter", catalogId, DbType.Int32);

        return await dapper.LoadDataAsync<T>(_getParameterInputByIdSql, parameters);
    }
}
