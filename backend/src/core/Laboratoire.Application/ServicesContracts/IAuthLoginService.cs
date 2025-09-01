using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IAuthLoginService
{
    public Task<Error> LoginUserAsync(UserLogin userLogin);
}
