using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IUserGetterByIdService
{
    Task<User?> GetUserByIdAsync(Guid? userId);
}
