using System.Data;
using Dapper;

using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Application.DTO;

namespace Laboratoire.Infrastructure.Repository;

public sealed class ProtocolRepository(DataContext dapper) : IProtocolRepository
{
    #region SQL queries
    private readonly string _getAllProtocolsSql =
    $"""
    SELECT 
        protocol_id AS {nameof(Protocol.ProtocolId)},
        cash_flow_id AS {nameof(Protocol.CashFlowId)},
        report_id AS {nameof(Protocol.ReportId)},
        client_id AS {nameof(Protocol.ClientId)},
        property_id AS {nameof(Protocol.PropertyId)},
        partner_id AS {nameof(Protocol.PartnerId)},
        catalog_id AS {nameof(Protocol.CatalogId)},
        entry_date AS {nameof(Protocol.EntryDate)},
        report_date AS {nameof(Protocol.ReportDate)},
        is_collected_by_client AS {nameof(Protocol.IsCollectedByClient)}
    FROM
        document.protocol;
    """;

    private readonly string _getProtocolByIdSql =
    $"""
    SELECT 
        protocol_id AS {nameof(Protocol.ProtocolId)},
        cash_flow_id AS {nameof(Protocol.CashFlowId)},
        report_id AS {nameof(Protocol.ReportId)},
        client_id AS {nameof(Protocol.ClientId)},
        property_id AS {nameof(Protocol.PropertyId)},
        partner_id AS {nameof(Protocol.PartnerId)},
        catalog_id AS {nameof(Protocol.CatalogId)},
        entry_date AS {nameof(Protocol.EntryDate)},
        report_date AS {nameof(Protocol.ReportDate)},
        is_collected_by_client AS {nameof(Protocol.IsCollectedByClient)}
    FROM 
        document.protocol
    WHERE 
        protocol_id = @ProtocolIdParameter;
    """;
    private readonly string _getProtocolByReportIdSql =
    $"""
    SELECT 
        protocol_id AS {nameof(Protocol.ProtocolId)},
        cash_flow_id AS {nameof(Protocol.CashFlowId)},
        report_id AS {nameof(Protocol.ReportId)},
        client_id AS {nameof(Protocol.ClientId)},
        property_id AS {nameof(Protocol.PropertyId)},
        partner_id AS {nameof(Protocol.PartnerId)},
        catalog_id AS {nameof(Protocol.CatalogId)},
        entry_date AS {nameof(Protocol.EntryDate)},
        report_date AS {nameof(Protocol.ReportDate)},
        is_collected_by_client AS {nameof(Protocol.IsCollectedByClient)}
    FROM 
        document.protocol
    WHERE 
        report_id = @ReportIdParameter;
    """;
    private readonly string _getUniqueProtocolSql =
    $"""
    SELECT 
        protocol_id AS {nameof(Protocol.ProtocolId)},
        cash_flow_id AS {nameof(Protocol.CashFlowId)},
        report_id AS {nameof(Protocol.ReportId)},
        client_id AS {nameof(Protocol.ClientId)},
        property_id AS {nameof(Protocol.PropertyId)},
        partner_id AS {nameof(Protocol.PartnerId)},
        catalog_id AS {nameof(Protocol.CatalogId)},
        entry_date AS {nameof(Protocol.EntryDate)},
        report_date AS {nameof(Protocol.ReportDate)},
        is_collected_by_client AS {nameof(Protocol.IsCollectedByClient)}
    FROM 
        document.protocol
    WHERE 
        client_id = @ClientIdParameter
        AND property_id = @PropertyIdParameter
        AND cash_flow_id = @CashFlowIdParameter;
    """;
    private readonly string _getProtocolYear =
    $"""
    SELECT DISTINCT 
        EXTRACT(YEAR FROM entry_date) AS Year
    FROM 
        document.protocol
    ORDER BY 
        Year;
    """;
    private readonly string _getDisplayProtocolsSql =
    $"""
    SELECT 
        p.protocol_id AS {nameof(Protocol.ProtocolId)},
        p.report_id AS {nameof(Protocol.ReportId)},
        p.entry_date AS {nameof(Protocol.EntryDate)},
        p.report_date As {nameof(Protocol.ReportDate)},
        p.is_collected_by_client AS {nameof(Protocol.IsCollectedByClient)},
        p.catalog_id AS {nameof(Protocol.CatalogId)},
        p.cash_flow_id AS {nameof(Protocol.CashFlowId)},

        cf.total_paid AS {nameof(CashFlow.TotalPaid)},
        cf.payment_date AS {nameof(CashFlow.PaymentDate)},
        cf.transaction_id AS {nameof(CashFlow.TransactionId)},

        c.client_id AS {nameof(Client.ClientId)},
        c.client_name AS {nameof(Client.ClientName)},
        c.client_tax_id AS {nameof(Client.ClientTaxId)},

        cp.property_id AS {nameof(Property.PropertyId)},
        cp.property_name AS {nameof(Property.PropertyName)},
        cp.city AS {nameof(Property.City)},
        cp.postal_code AS {nameof(Property.PostalCode)},
        cp.area AS {nameof(Property.Area)},
        cp.ccir AS {nameof(Property.Ccir)},
        cp.itr_nirf AS {nameof(Property.ItrNirf)},

        r.results AS {nameof(Report.Results)},

        pa.partner_id AS {nameof(Partner.PartnerId)},
        pa.partner_name AS {nameof(Partner.PartnerName)},

        ca.report_type AS {nameof(Catalog.ReportType)},
        ca.price AS {nameof(Catalog.Price)},

        s.state_id AS {nameof(State.StateId)},
        s.state_code AS {nameof(State.StateCode)}
    FROM 
        document.protocol AS p
    LEFT JOIN   
        cash_flow.cash_flow AS cf 
        USING(cash_flow_id)
    INNER JOIN 
        customers.client AS c 
        ON p.client_id = c.client_id
    INNER JOIN 
        customers.property AS cp 
        ON p.property_id = cp.property_id
    LEFT JOIN 
        document.report AS r 
        ON p.report_id = r.report_id
    LEFT JOIN 
        customers.partner AS pa 
        ON p.partner_id = pa.partner_id
    INNER JOIN 
        parameters.catalog AS ca 
        ON p.catalog_id = ca.catalog_id
    INNER JOIN 
        utils.state AS s 
        ON cp.state_id = s.state_id
    WHERE 
        EXTRACT(YEAR FROM p.entry_date) = @YearParameter
    ORDER BY 
        p.protocol_id;
    """;
    private readonly string _addProtocolSql =
    $"""
    INSERT INTO document.protocol(
        cash_flow_id,
        report_id,
        client_id,
        property_id,
        partner_id,
        catalog_id,
        entry_date,
        report_date,
        is_collected_by_client
    )
    VALUES
    (
        @CashFlowIdParameter,
        @ReportIdParameter,
        @ClientIdParameter,
        @PropertyIdParameter,
        @PartnerIdParameter,
        @CatalogIdParameter,
        @EntryDateParameter,
        @ReportDateParameter,
        @IsCollectedByClientParameter
    )
    RETURNING 
        protocol_id;
    """;
    private readonly string _updateProtocolSql =
    $"""
    UPDATE document.protocol
    SET
        client_id = @ClientIdParameter,
        property_id = @PropertyIdParameter,
        partner_id = @PartnerIdParameter,
        entry_date = @EntryDateParameter,
        report_date = @ReportDateParameter,
        is_collected_by_client = @IsCollectedByClientParameter
    WHERE 
        protocol_id = @ProtocolIdParameter;
    """;

    private readonly string _patchReportIdSql =
    $"""
    UPDATE document.protocol
    SET
        report_id = @ReportIdParameter
    WHERE
        protocol_id = @ProtocolIdParameter;
    """;
    private readonly string _countProtocol =
    $"""
    SELECT
        COUNT(protocol_id)
    FROM 
        document.protocol
    WHERE
        protocol_id = @ProtocolIdParameter;
    """;
    private readonly string _updateCatalogSql =
    $"""
    UPDATE document.protocol
    SET
        catalog_id = @CatalogIdParameter
    WHERE 
        protocol_id = @ProtocolIdParameter;
    """;
    private readonly string _updateCashFlowIdSql =
    $"""
    UPDATE document.protocol
    SET
        cash_flow_id = @CashFlowIdParameter
    WHERE 
        protocol_id = @ProtocolIdParameter;
    """;
    #endregion
    public async Task<string?> AddProtocolAsync(Protocol protocol)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@CashFlowIdParameter", protocol.CashFlowId, DbType.Int32);
        parameters.Add("@ReportIdParameter", protocol.ReportId, DbType.Guid);
        parameters.Add("@ClientIdParameter", protocol.ClientId, DbType.Guid);
        parameters.Add("@PropertyIdParameter", protocol.PropertyId, DbType.Int32);
        parameters.Add("@PartnerIdParameter", protocol.PartnerId, DbType.Guid);
        parameters.Add("@CatalogIdParameter", protocol.CatalogId, DbType.Int32);
        parameters.Add("@EntryDateParameter", protocol.EntryDate, DbType.Date);
        parameters.Add("@ReportDateParameter", protocol.ReportDate, DbType.Date);
        parameters.Add("@IsCollectedByClientParameter", protocol.IsCollectedByClient, DbType.Boolean);

        return await dapper.LoadDataSingleAsync<string>(_addProtocolSql, parameters);
    }
    public async Task<bool> DoesProtocolExistByProtocolIdAsync(Protocol protocol)
    => await GetProtocolByProtocolIdAsync(protocol.ProtocolId) is not null;
    public async Task<bool> DoesProtocolExistByProtocolIdAsync(string? protocolId)
    => await GetProtocolByProtocolIdAsync(protocolId) is not null;
    public async Task<bool> DoesProtocolExistByReportIdAsync(Guid? reportId)
    => await GetProtocolByReportIdAsync(reportId) is not null;
    public async Task<bool> DoesProtocolExistByUniqueAsync(Protocol protocol)
    => await GetUniqueProtocolAsync(protocol) is not null;
    public async Task<IEnumerable<Protocol>> GetAllProtocolsAsync()
    => await dapper.LoadDataAsync<Protocol>(_getAllProtocolsSql);

    //FIXME:Remove the year 
    // public async Task<IEnumerable<T>> GetDisplayProtocolsAsync<T>(int year)
    // {
    //     DynamicParameters parameters = new DynamicParameters();
    //     parameters.Add("@YearParameter", year, DbType.Int32);

    //     return await dapper.LoadDataAsync<T>(_getDisplayProtocolsSql, parameters);
    // }
    public async Task<IEnumerable<T>> GetDisplayProtocolsAsync<T>(int year, bool isEmployee)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@YearParameter", isEmployee ? year : 2025, DbType.Int32);

        return await dapper.LoadDataAsync<T>(_getDisplayProtocolsSql, parameters);

    }
    public async Task<Protocol?> GetProtocolByProtocolIdAsync(string? protocolId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ProtocolIdParameter", protocolId, DbType.String);

        return await dapper.LoadDataSingleAsync<Protocol>(_getProtocolByIdSql, parameters);
    }

    public async Task<Protocol?> GetProtocolByReportIdAsync(Guid? reportId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ReportIdParameter", reportId, DbType.Guid);

        return await dapper.LoadDataSingleAsync<Protocol>(_getProtocolByReportIdSql, parameters);
    }
    public Task<IEnumerable<T>> GetProtocolYearsAsync<T>()
    => dapper.LoadDataAsync<T>(_getProtocolYear);
    public async Task<Protocol?> GetUniqueProtocolAsync(Protocol protocol)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ClientIdParameter", protocol.ClientId, DbType.Guid);
        parameters.Add("@PropertyIdParameter", protocol.PropertyId, DbType.Int32);
        parameters.Add("@CashFlowIdParameter", protocol.CashFlowId, DbType.Int32);

        return await dapper.LoadDataSingleAsync<Protocol>(_getUniqueProtocolSql, parameters);
    }
    public async Task<bool> IsProtocolDoubled(string? protocolId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ProtocolIdParameter", protocolId, DbType.String);

        return await dapper.LoadDataSingleAsync<int>(_countProtocol, parameters) > 1;
    }
    public async Task<bool> PatchReportIdAsync(ReportPatch reportPatch)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ReportIdParameter", reportPatch?.ReportId, DbType.Guid);
        parameters.Add("@ProtocolIdParameter", reportPatch?.ProtocolId, DbType.String);

        return await dapper.ExecuteSqlAsync(_patchReportIdSql, parameters);
    }
    public async Task<bool> UpdateCatalogAsync(Protocol protocol)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@CatalogIdParameter", protocol.CatalogId, DbType.Int32);
        parameters.Add("@ProtocolIdParameter", protocol.ProtocolId, DbType.String);

        return await dapper.ExecuteSqlAsync(_updateCatalogSql, parameters);
    }
    public async Task<bool> UpdateProtocolAsync(Protocol protocol)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ReportIdParameter", protocol.ReportId, DbType.Guid);
        parameters.Add("@ClientIdParameter", protocol.ClientId, DbType.Guid);
        parameters.Add("@PropertyIdParameter", protocol.PropertyId, DbType.Int32);
        parameters.Add("@PartnerIdParameter", protocol.PartnerId, DbType.Guid);
        parameters.Add("@EntryDateParameter", protocol.EntryDate, DbType.Date);
        parameters.Add("@ReportDateParameter", protocol.ReportDate, DbType.Date);
        parameters.Add("@IsCollectedByClientParameter", protocol.IsCollectedByClient, DbType.Boolean);
        parameters.Add("@ProtocolIdParameter", protocol.ProtocolId, DbType.String);

        return await dapper.ExecuteSqlAsync(_updateProtocolSql, parameters);
    }
    public async Task<bool> UpdateCashFlowIdAsync(Protocol protocol)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@CashFlowIdParameter", protocol.CashFlowId, DbType.Int32);
        parameters.Add("@ProtocolIdParameter", protocol.ProtocolId, DbType.String);

        return await dapper.ExecuteSqlAsync(_updateCashFlowIdSql, parameters);
    }
    public async Task<bool> SaveProtocolSpotAsync(int? quantity, Guid? clientId, int propertyId)
    {
        var protocol = new List<ProtocolDtoAddBulk>();
        var entryDate = DateTime.Now;
        var reportDate = DateTime.Now.AddDays(3);
        for (int i = 0; i < quantity; i++)
        {
            var tempProtocol = new ProtocolDtoAddBulk()
            {
                CashFlowId = null,
                ReportId = null,
                ClientId = clientId,
                PropertyId = propertyId,
                PartnerId = null,
                CatalogId = 2,
                EntryDate = entryDate,
                ReportDate = reportDate,
                IsCollectedByClient = true
            };
            protocol.Add(tempProtocol);
        }
        var (sql, parameters) = Utils.Utils.BulkSqlStatement(protocol, "protocol", "document");

        return await dapper.ExecuteSqlAsync(sql, parameters);
    }
}
