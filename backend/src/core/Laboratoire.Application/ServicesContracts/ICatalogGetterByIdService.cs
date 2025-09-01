using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface ICatalogGetterByIdService
{
    Task<Catalog?> GetCatalogByIdAsync(int? catalogId);
}
