using Laboratoire.Application.DTO;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.Mapper;

public static class OfficeMapper
{
    public static Office ToOffice(this OfficeDtoUpsert dto)
    => new Office()
    {
        OfficeId = default,
        OfficeName = dto.OfficeName,
        OfficeEmail = dto.OfficeEmail,
        City = dto.City,
    };
    public static Office ToOffice(this OfficeDtoUpsert dto, Guid officeId)
    => new Office()
    {
        OfficeId = officeId,
        OfficeName = dto.OfficeName,
        OfficeEmail = dto.OfficeEmail,
        City = dto.City,
    };
}
