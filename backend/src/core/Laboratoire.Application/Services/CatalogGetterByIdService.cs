using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class CatalogGetterByIdService
(
    ICatalogRepository catalogRepository,
    ILogger<CatalogGetterByIdService> logger
)
: ICatalogGetterByIdService
{
    public async Task<Catalog?> GetCatalogByIdAsync(int? catalogId)
    {
        if (catalogId is null)
        {
            logger.LogWarning("GetCatalogByIdAsync was called with null catalogId.");
            return null;
        }

        logger.LogInformation("Retrieving catalog with ID: {CatalogId}", catalogId);
        return await catalogRepository.GetCatalogByIdAsync(catalogId);
    }
}
