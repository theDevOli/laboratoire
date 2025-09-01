using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface IUserAdderService
{
    Task<Guid?> AddUserAsync(UserDtoAdd userDto);
}
