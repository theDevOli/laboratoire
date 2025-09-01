using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IPermissionGetterService
{
    Task<IEnumerable<DisplayPermission>> GetAllPermissionsAsync();
}
