using Laboratoire.Application.DTO;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.Mapper;

public static class RoleMapper
{
    public static Role ToRole(this RoleDtoAdd dto)
    => new Role()
    {
        RoleId = default,
        RoleName = dto.RoleName?.Trim()
    };
}
