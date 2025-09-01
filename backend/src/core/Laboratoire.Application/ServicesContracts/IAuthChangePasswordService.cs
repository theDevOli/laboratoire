using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface IAuthChangePasswordService
{
    Task<Error> ChangeUserPasswordAsync(UserDtoChangePassword userDto);
}
