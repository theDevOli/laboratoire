
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.ParameterServices;

public class ParameterGetterService
(
    IParameterRepository parameterRepository,
    ILogger<ParameterGetterService> logger
)
: IParameterGetterService
{
    public Task<IEnumerable<Parameter>> GetAllParametersAsync()
    {
        logger.LogInformation("Fetching all parameters from repository.");

        return parameterRepository.GetAllParametersAsync();
    }
}
