using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IRoleGetterByIdService
{
    Task<Role?> GetRoleByIdAsync(int? roleId);
}
