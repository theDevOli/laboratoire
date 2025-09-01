using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface ICatalogAdderService
{
    Task<Error> AddCatalogAsync(CatalogDtoAdd catalog);
}
