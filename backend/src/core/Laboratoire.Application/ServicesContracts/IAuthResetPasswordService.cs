using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IAuthResetPasswordService
{
    Task<Error> ResetPasswordAsync(Guid? userId);
}
