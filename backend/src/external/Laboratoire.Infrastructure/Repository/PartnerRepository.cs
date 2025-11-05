using System.Data;
using Dapper;

using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;

namespace Laboratoire.Infrastructure.Repository;

public sealed class PartnerRepository(DataContext dapper) : IPartnerRepository
{
    #region SQL queries
    private readonly string _getAllPartners =
    $"""
    SELECT
        partner_id AS {nameof(Partner.PartnerId)},
        office_id AS {nameof(Partner.OfficeId)},
        user_id AS {nameof(Partner.UserId)},
        partner_name AS {nameof(Partner.PartnerName)},
        partner_phone AS {nameof(Partner.PartnerPhone)}
    FROM 
        customers.partner
    ORDER BY
        partner_name;
    """;
    private readonly string _getActivePartners =
    $"""
    SELECT
        cp.partner_id AS {nameof(Partner.PartnerId)},
        user_id AS {nameof(Partner.UserId)},
        cp.partner_name AS {nameof(Partner.PartnerName)},
        cp.office_id AS {nameof(Partner.OfficeId)},
        cp.partner_phone AS {nameof(Partner.PartnerPhone)}
    FROM 
        customers.partner AS cp
    INNER JOIN
        users.user AS u 
        ON cp.user_id = u.user_id
    WHERE
        u.is_active = true
    ORDER BY
        cp.partner_name;
    """;
    private readonly string _getPartnerById =
    $"""
    SELECT 
        partner_id AS {nameof(Partner.PartnerId)},
        user_id AS {nameof(Partner.UserId)},
        partner_name AS {nameof(Partner.PartnerName)},
        office_id AS {nameof(Partner.OfficeId)},
        partner_phone AS {nameof(Partner.PartnerPhone)}
    FROM 
        customers.partner
    WHERE 
        partner_id = @PartnerIdParameter;
    """;
    private readonly string _getPartnerByOfficeAndName =
    $"""
    SELECT 
        partner_id AS {nameof(Partner.PartnerId)},
        user_id AS {nameof(Partner.UserId)},
        office_id AS {nameof(Partner.OfficeId)},
        partner_name AS {nameof(Partner.PartnerName)},
        partner_phone AS {nameof(Partner.PartnerPhone)}
    FROM 
        customers.partner
    WHERE 
        partner_name = @PartnerNameParameter
        AND partner_email = @PartnerEmailParameter;
    """;
    private readonly string _addPartner =
    $"""
    INSERT INTO customers.partner(
        partner_name,
        office_id,
        partner_phone,
        user_id
    )
    VALUES(
        @PartnerNameParameter,
        @OfficeIdParameter,
        @PartnerPhoneParameter,
        @UserIdParameter
    );
    """;
    private readonly string _updatePartner =
    $"""
    UPDATE customers.partner
    SET
        partner_name = @PartnerNameParameter,
        office_id= @OfficeIdParameter,
        partner_phone = @PartnerPhoneParameter,
        partner_email = @PartnerEmailParameter
    WHERE 
        partner_id = @PartnerIdParameter;
    """;
    #endregion
    public async Task<bool> AddPartnerAsync(Partner partner, Guid? userId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@PartnerNameParameter", partner.PartnerName, DbType.String);
        parameters.Add("@OfficeIdParameter", partner.OfficeId, DbType.Guid);
        parameters.Add("@PartnerPhoneParameter", partner.PartnerPhone, DbType.String);
        parameters.Add("@UserIdParameter", userId, DbType.Guid);

        return await dapper.ExecuteSqlAsync(_addPartner, parameters);
    }
    public async Task<bool> DoesPartnerExistByIdAsync(Partner partner)
    => await GetPartnerByIdAsync(partner.PartnerId) is not null;
    public async Task<bool> DoesPartnerExistByOfficeAndNameAsync(Partner partner)
    => await GetPartnerByOfficeAndNameAsync(partner) is not null;
    public async Task<IEnumerable<Partner>> GetAllPartnersAsync()
    => await dapper.LoadDataAsync<Partner>(_getAllPartners);
    public async Task<Partner?> GetPartnerByOfficeAndNameAsync(Partner partner)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@PartnerNameParameter", partner.PartnerName, DbType.String);
        parameters.Add("@OfficeIdParameter", partner.OfficeId, DbType.Guid);

        return await dapper.LoadDataSingleAsync<Partner>(_getPartnerByOfficeAndName, parameters);
    }
    public async Task<Partner?> GetPartnerByIdAsync(Guid? partnerId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@PartnerIdParameter", partnerId, DbType.Guid);

        return await dapper.LoadDataSingleAsync<Partner>(_getPartnerById, parameters);
    }
    public async Task<bool> UpdatePartnerAsync(Partner partner)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@PartnerNameParameter", partner.PartnerName, DbType.String);
        parameters.Add("@OfficeIdParameter", partner.OfficeId, DbType.Guid);
        parameters.Add("@PartnerPhoneParameter", partner.PartnerPhone, DbType.String);
        parameters.Add("@PartnerIdParameter", partner.PartnerId, DbType.Guid);

        return await dapper.ExecuteSqlAsync(_updatePartner, parameters);
    }
    public async Task<IEnumerable<Partner>> GetActivePartnersAsync()
    => await dapper.LoadDataAsync<Partner>(_getActivePartners);
}
