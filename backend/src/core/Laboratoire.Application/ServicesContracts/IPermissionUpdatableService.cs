
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IPermissionUpdatableService
{
    Task<Error> UpdatePermissionAsync(Permission permission);
}
