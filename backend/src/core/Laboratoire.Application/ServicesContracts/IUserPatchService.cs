using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface IUserPatchService
{
    Task<Error> UpdateUserStatusAsync(Guid? userId);
}
