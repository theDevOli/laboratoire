using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface IRoleAdderService
{
    Task<Error> AddRoleAsync(RoleDtoAdd roleDto);
}
