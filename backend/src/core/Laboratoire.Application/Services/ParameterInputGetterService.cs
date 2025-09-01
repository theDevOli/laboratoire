using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class ParameterInputGetterService
(
    IParameterRepository parameterRepository,
    ILogger<ParameterInputGetterService> logger
)
: IParameterInputGetterService
{
    public async Task<IEnumerable<ParameterDtoDisplay>?> GetParameterInputByIdAsync(int? catalogId)
    {
        if (catalogId is null)
        {
            logger.LogWarning("GetParameterInputByIdAsync called with null catalogId.");
            return null;
        }

        logger.LogInformation("Fetching parameter inputs for catalogId: {CatalogId}", catalogId);

        return await parameterRepository.GetParametersInputByCatalogIdAsync<ParameterDtoDisplay>(catalogId);
    }
}
