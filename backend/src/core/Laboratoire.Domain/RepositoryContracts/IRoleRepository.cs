
using Laboratoire.Domain.Entity;

namespace Laboratoire.Domain.RepositoryContracts;

public interface IRoleRepository
{
    Task<IEnumerable<Role>> GetAllRolesAsync();
    Task<Role?> GetRoleByIdAsync(int? roleId);
    Task<Role?> GetRoleByNameAsync(string? roleName);
    Task<string?> GetRoleNameByUserIdAsync(Guid? userId);
    Task<bool> DoesRoleExistByIdAsync(Role role);
    Task<bool> DoesRoleExistByNameAsync(Role role);
    Task<bool> AddRoleAsync(Role role);
    Task<bool> UpdateRoleAsync(Role role);
}
