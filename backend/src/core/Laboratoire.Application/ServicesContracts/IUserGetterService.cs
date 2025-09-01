using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IUserGetterService
{
    Task<IEnumerable<DisplayUser>> GetAllUsersAsync();
}
