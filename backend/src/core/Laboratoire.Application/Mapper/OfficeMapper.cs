using Laboratoire.Application.DTO;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.Mapper;

public static class OfficeMapper
{
    public static Office ToOffice(this OfficeDtoAdd dto)
    => new Office()
    {
        OfficeId = default,
        OfficeName = dto.OfficeName,
        OfficeEmail = dto.OfficeEmail,
        City = dto.City,
    };
}
