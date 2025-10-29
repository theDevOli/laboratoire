using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IOfficeAdderService
{
    public Task<Error> AddOfficeAsync(Office office);
}
