using Laboratoire.Application.DTO;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.Mapper;

public static class PartnerMapper
{
    public static Partner ToPartner(this PartnerDtoAdd dto)
    => new Partner()
    {
        PartnerId = default,
        PartnerName = dto.PartnerName?.Trim(),
        OfficeName = dto.OfficeName?.Trim(),
        PartnerPhone = dto.PartnerPhone?.Trim(),
        PartnerEmail = dto.PartnerEmail?.Trim(),
    };
    public static UserDtoAdd ToUser(this PartnerDtoAdd dto)
    => new UserDtoAdd()
    {
        RoleId = 4,
        Username = dto.Username?.Trim(),
        IsActive = dto.IsActive,
    };
}
