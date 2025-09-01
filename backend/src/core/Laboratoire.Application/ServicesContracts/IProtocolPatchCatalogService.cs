using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IProtocolPatchCatalogService
{
    Task<Error> UpdateCatalogAsync(Protocol protocol);
}
