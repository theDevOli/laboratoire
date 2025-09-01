using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IAuthenticationGetterService
{
    Task<Authentication?> GetAuthenticationByUserId(Guid? userId);
}
