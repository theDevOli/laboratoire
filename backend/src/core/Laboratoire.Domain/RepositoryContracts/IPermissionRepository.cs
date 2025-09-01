using Laboratoire.Domain.Entity;

namespace Laboratoire.Domain.RepositoryContracts;

public interface IPermissionRepository
{

    Task<IEnumerable<DisplayPermission>> GetAllPermissionsAsync();
    Task<Permission?> GetPermissionByPermissionIdAsync(int? PermissionId);
    Task<Permission?> GetPermissionByRoleIdAsync(int? roleId);
    Task<bool> DoesPermissionExistByPermissionIdAsync(Permission permission);
    Task<bool> DoesPermissionExistByRoleIdAsync(Permission permission);
    Task<bool> AddPermissionAsync(Permission permission);
    Task<bool> UpdatePermissionAsync(Permission permission);
}
