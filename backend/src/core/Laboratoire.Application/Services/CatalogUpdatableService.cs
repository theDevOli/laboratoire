using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class CatalogUpdatableService
(
    ICatalogRepository catalogRepository,
    ILogger<CatalogUpdatableService> logger
)
: ICatalogUpdatableService
{
    public async Task<Error> UpdateCatalogAsync(Catalog catalog)
    {
        logger.LogInformation("Checking existence of catalog with ID {CatalogId}", catalog.CatalogId);
        var exists = await catalogRepository.DoesCatalogExistByIdAsync(catalog);
        if (!exists)
        {
            logger.LogWarning("Catalog with CatalogId {CatalogId} not found", catalog.CatalogId);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        logger.LogInformation("Updating catalog with ID {CatalogId}", catalog.CatalogId);

        var ok = await catalogRepository.UpdateCatalogAsync(catalog);
        if (!ok)
        {
            logger.LogError("Failed to update catalog with ID {CatalogId}", catalog.CatalogId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Catalog with ID {CatalogId} updated successfully", catalog.CatalogId);
        return Error.SetSuccess();
    }
}
