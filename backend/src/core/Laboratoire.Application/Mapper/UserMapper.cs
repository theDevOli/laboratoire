using Laboratoire.Application.DTO;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.Mapper;

public static class UserMapper
{
    public static User ToUser(this UserDtoAdd dto)
    => new User()
    {
        UserId = default,
        RoleId = dto.RoleId,
        Username = dto.Username?.Trim(),
        IsActive = dto.IsActive,
    };

    public static User ToUser(this UserDtoRename dto)
    => new User()
    {
        UserId = dto.UserId,
        RoleId = default,
        IsActive = default,
    };
    
    public static UserDtoRename ToUserRename(this User user)
        => new UserDtoRename()
        {
            UserId = user.UserId,
            Username = user.Username,
        };
}

