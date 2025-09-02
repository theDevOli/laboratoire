using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.CatalogServices;

public class CatalogAdderService
(
    ICatalogRepository catalogRepository,
    ILogger<CatalogAdderService> logger
)
: ICatalogAdderService
{
    public async Task<Error> AddCatalogAsync(CatalogDtoAdd catalogDto)
    {
        logger.LogInformation("Attempting to add new catalog: {LabelName}", catalogDto.LabelName);
        var catalog = catalogDto.ToCatalog();
        var exists = await catalogRepository.DoesCatalogExistByUniqueAsync(catalog);
        if (exists)
        {
            logger.LogWarning("Catalog with unique fields already exists:  {LabelName}", catalogDto.LabelName);
            return Error.SetError(ErrorMessage.ConflictPost, 409);
        }

        var ok = await catalogRepository.AddCatalogAsync(catalog);
        if (!ok)
        {
            logger.LogError("Failed to insert new catalog into the database: {LabelName}", catalogDto.LabelName);
            return Error.SetError(ErrorMessage.ConflictPost, 409);
        }

        logger.LogInformation("Catalog successfully added: {LabelName}", catalogDto.LabelName);
        return Error.SetSuccess();
    }
}
