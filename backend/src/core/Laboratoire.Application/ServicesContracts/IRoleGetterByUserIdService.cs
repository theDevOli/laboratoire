namespace Laboratoire.Application.ServicesContracts;

public interface IRoleGetterByUserIdService
{
    Task<string?> GetRoleNameByUserIdAsync(Guid? userId);
}
