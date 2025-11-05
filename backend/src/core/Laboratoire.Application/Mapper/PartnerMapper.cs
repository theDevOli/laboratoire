using Laboratoire.Application.DTO;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.Mapper;

public static class PartnerMapper
{
    public static Partner ToPartner(this PartnerDtoAdd dto)
    => new Partner()
    {
        PartnerId = default,
        OfficeId = dto.OfficeId,
        PartnerName = dto.PartnerName?.Trim(),
        PartnerPhone = dto.PartnerPhone?.Trim(),
    };

    public static UserDtoAdd ToUser(this PartnerDtoAdd dto)
    => new()
    {
        RoleId = 4,
        Username = dto.Username?.Trim(),
        IsActive = dto.IsActive,
        Client = default,
        Partner = dto.ToPartner(),
    };
}
