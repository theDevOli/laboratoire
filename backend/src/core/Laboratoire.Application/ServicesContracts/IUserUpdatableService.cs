using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IUserUpdatableService
{
    Task<Error> UpdateUserAsync(User user);
}
