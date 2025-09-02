using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.ParameterServices;

public class ParameterGetterByIdService
(
    IParameterRepository parameterRepository,
    ILogger<ParameterGetterByIdService> logger
)
: IParameterGetterByIdService
{
    public async Task<Parameter?> GetParameterByIdAsync(int? parameterId)
    {
        if (parameterId is null)
        {
            logger.LogWarning("GetParameterByIdAsync called with null parameterId.");
            return null;
        }

        logger.LogInformation("Fetching parameter with ID: {ParameterId}", parameterId);
        return await parameterRepository.GetParameterByParameterIdAsync(parameterId);
    }
}
