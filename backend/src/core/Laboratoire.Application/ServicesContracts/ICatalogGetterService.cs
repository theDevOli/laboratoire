using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface ICatalogGetterService
{
    Task<IEnumerable<Catalog>> GetAllCatalogsAsync();
}
