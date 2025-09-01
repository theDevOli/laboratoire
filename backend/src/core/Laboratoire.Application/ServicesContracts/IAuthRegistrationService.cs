using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IAuthRegistrationService
{
    public Task<Error> RegisterUserAsync(UserRegistration user);
}
