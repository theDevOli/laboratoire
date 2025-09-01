using Laboratoire.Application.DTO;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.Mapper;

public static class PermissionMapper
{
    public static Permission ToPermission(this PermissionDtoAdd dto)
    => new Permission()
    {
        PermissionId = default,
        RoleId = dto.RoleId,
        Protocol = dto.Protocol,
        Client = dto.Client,
        Property = dto.Property,
        CashFlow = dto.CashFlow,
        Partner = dto.Partner,
        Users = dto.Users,
        Chemical = dto.Chemical,
    };
}
