using System;
using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface IUserRenameService
{
    Task<Error> UserRenameAsync(UserDtoRename userDto);

}
