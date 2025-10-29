using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IOfficeUpdatableService
{
    public Task<Error> UpdateOfficeAsync(Office office);
}
