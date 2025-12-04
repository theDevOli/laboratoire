using Laboratoire.Application.DTO;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.Mapper;

public static class PartnerMapper
{
    public static Partner ToPartner(this PartnerDtoUpsert dto)
    => new Partner()
    {
        PartnerId = default,
        OfficeId = dto.OfficeId,
        PartnerName = dto.PartnerName?.Trim(),
        PartnerPhone = dto.PartnerPhone?.Trim(),
    };
    public static Partner ToPartner(this PartnerDtoUpsert dto,Guid partnerId)
    => new Partner()
    {
        PartnerId = partnerId,
        OfficeId = dto.OfficeId,
        PartnerName = dto.PartnerName?.Trim(),
        PartnerPhone = dto.PartnerPhone?.Trim(),
    };

    public static UserDtoAdd ToUser(this PartnerDtoUpsert dto)
    => new()
    {
        RoleId = 4,
        Username = default,
        IsActive = true,
        Client = default,
        Partner = dto.ToPartner(),
    };
}
