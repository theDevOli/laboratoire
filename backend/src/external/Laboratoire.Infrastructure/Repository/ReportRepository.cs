using System.Data;
using Dapper;

using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Application.DTO;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Application.Mapper;

namespace Laboratoire.Infrastructure.Repository;

 sealed class ReportRepository(DataContext dapper) : IReportRepository
{
    #region SQL queries
    private readonly string _getAllReports =
    $"""
    SELECT
        report_id AS {nameof(Report.ReportId)},
        protocol_id AS {nameof(Report.ProtocolId)},
        results AS {nameof(Report.Results)}
    FROM 
        document.report;
    """;
    private readonly string _getReportById =
    $"""
    SELECT
        report_id AS {nameof(Report.ReportId)},
        protocol_id AS {nameof(Report.ProtocolId)},
        results AS {nameof(Report.Results)}
    FROM 
        document.report
    WHERE 
        report_id = @ReportIdParameter;
    """;
    private readonly string _addReportSql =
    $"""
    INSERT INTO document.report (
        protocol_id,
        results
    )
    VALUES 
    (
        @ProtocolIdParameter,
        @ResultsParameter ::JSONB[]
    )
    RETURNING 
        report_id;
    """;
    private readonly string _patchReportSql =
    $"""
    UPDATE document.report
    SET
        results = @ResultsParameter ::JSONB[]
    WHERE
        report_id = @ReportIdParameter;
    """;
    private readonly string _getPDFReportSql =
    $"""
    SELECT
        pr.entry_date AS {nameof(Protocol.EntryDate)},
        pr.report_date AS {nameof(Protocol.ReportDate)},
        pr.is_collected_by_client AS {nameof(Protocol.IsCollectedByClient)},

        r.protocol_id AS {nameof(Report.ProtocolId)},
        r.results AS {nameof(Report.Results)},

        c.client_name AS {nameof(Client.ClientName)},
        c.client_tax_id AS {nameof(Client.ClientTaxId)},
        c.client_email AS {nameof(Client.ClientEmail)},
        c.client_phone AS {nameof(Client.ClientPhone)},

        p.property_name AS {nameof(Property.PropertyName)},
        p.city AS {nameof(Property.City)},
        p.area AS {nameof(Property.Area)},
        p.ccir AS {nameof(Property.Ccir)},
        p.itr_nirf AS {nameof(Property.ItrNirf)},
        p.registration AS {nameof(Property.Registration)},

        ca.report_type AS {nameof(Catalog.ReportType)},
        ca.sample_type AS {nameof(Catalog.SampleType)},
        ca.catalog_id AS {nameof(Catalog.CatalogId)},
        ca.legends AS {nameof(Catalog.Legends)},

        s.state_code AS {nameof(State.StateCode)}
        FROM 
            document.protocol AS pr
        INNER JOIN 
            document.report AS r 
            USING (report_id)
        INNER JOIN 
            customers.client AS c
            USING (client_id)
        INNER JOIN 
            customers.property AS p 
            USING (property_id)
        INNER JOIN 
            parameters.catalog AS ca
            USING(catalog_id)
        INNER JOIN 
            utils.state AS s
            ON p.state_id = s.state_id
        WHERE 
            r.report_id = @ReportIdParameter;
    """;
    private readonly string _countProtocol =
    $"""
    SELECT 
        COUNT(*)
    FROM
        document.report
    WHERE
        protocol_id = @ProtocolIdParameter;
    """;
    private readonly string _resetReportSql =
    $"""
    UPDATE document.report
    SET
        results = null
    WHERE 
        report_id = @ReportIdParameter;
    """;
    #endregion
    public async Task<Guid?> AddReportAsync(Report report)
    {
        var reportDto = report.ToDb();

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ProtocolIdParameter", reportDto.ProtocolId, DbType.String);
        parameters.Add("@ResultsParameter", reportDto.Results, DbType.Object);

        return await dapper.LoadDataSingleAsync<Guid>(_addReportSql, parameters);
    }
    public async Task<bool> DoesReportExistsAsync(Report report)
    => await GetReportByIdAsync(report.ReportId) is not null;
    public async Task<IEnumerable<Report>> GetAllReportsAsync()
    {
        var reports = await dapper.LoadDataAsync<ReportDtoDb>(_getAllReports);

        return reports.Select(res => res.FromDb());
    }
    public async Task<Report?> GetReportByIdAsync(Guid? reportId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ReportIdParameter", reportId, DbType.Guid);

        var report = await dapper.LoadDataSingleAsync<ReportDtoDb>(_getReportById, parameters);

        if (report is null)
            return null;

        return report.FromDb();
    }
    public async Task<ReportPDF?> GetReportPDFAsync(Guid? reportId)
    {
        if (reportId is null)
            return null;
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ReportIdParameter", reportId, DbType.Guid);

        var reportDto = await dapper.LoadDataSingleAsync<ReportDtoPDFDb>(_getPDFReportSql, parameters);
        return reportDto?.FromDb();
    }
    public async Task<bool> IsProtocolDoubled(Report report)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ProtocolIdParameter", report.ProtocolId, DbType.String);

        return await dapper.LoadDataSingleAsync<int>(_countProtocol, parameters) > 1;
    }
    public async Task<bool> PatchReportAsync(Report report)
    {
        var reportDto = report.ToDb();

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ResultsParameter", reportDto.Results, DbType.Object);
        parameters.Add("@ReportIdParameter", reportDto.ReportId, DbType.Guid);

        return await dapper.ExecuteSqlAsync(_patchReportSql, parameters);
    }
    public async Task<bool> ResetReportAsync(Guid? reportId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ReportIdParameter", reportId, DbType.Guid);

        return await dapper.ExecuteSqlAsync(_resetReportSql, parameters);
    }
}

