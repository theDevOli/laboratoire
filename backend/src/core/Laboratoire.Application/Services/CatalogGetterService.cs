using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class CatalogGetterService
(
    ICatalogRepository catalogRepository,
    ILogger<CatalogGetterService> logger
)
: ICatalogGetterService
{
    public  Task<IEnumerable<Catalog>> GetAllCatalogsAsync()
    {
        logger.LogInformation("Retrieving all catalogs");
        return catalogRepository.GetAllCatalogsAsync();
    }
}
