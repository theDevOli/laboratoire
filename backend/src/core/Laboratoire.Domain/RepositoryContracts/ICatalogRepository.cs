using Laboratoire.Domain.Entity;

namespace Laboratoire.Domain.RepositoryContracts;

public interface ICatalogRepository
{
    Task<IEnumerable<Catalog>> GetAllCatalogsAsync();
    Task<Catalog?> GetCatalogByIdAsync(int? catalogId);
    Task<Catalog?> GetUniqueCatalogAsync(Catalog catalog);
    Task<bool> DoesCatalogExistByIdAsync(Catalog catalog);
    Task<bool> DoesCatalogExistByUniqueAsync(Catalog catalog);
    Task<bool> AddCatalogAsync(Catalog catalog);
    Task<bool> UpdateCatalogAsync(Catalog catalog);
}
