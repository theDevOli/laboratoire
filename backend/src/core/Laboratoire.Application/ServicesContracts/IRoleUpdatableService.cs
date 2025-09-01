using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IRoleUpdatableService
{
    Task<Error> UpdateRoleAsync(Role role);
}
