using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IOfficeGetterService
{
    public Task<IEnumerable<Office>> GetAllOfficesAsync();
}
