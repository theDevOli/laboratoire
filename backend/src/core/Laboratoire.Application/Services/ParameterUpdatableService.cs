using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class ParameterUpdatableService
(
    IParameterRepository parameterRepository,
    ILogger<ParameterUpdatableService> logger
)
: IParameterUpdatableService
{
    public async Task<Error> UpdateParameterAsync(Parameter parameter)
    {
        logger.LogInformation("Starting update for parameter ID: {ParameterId}, Name: {ParameterName}", parameter.ParameterId, parameter.ParameterName);
        
        var isConflict = await parameterRepository.IsParameterUniqueAsync(parameter);
        if (isConflict)
        {
            logger.LogWarning("Conflict detected: Parameter with similar attributes already exists.");
            return Error.SetError(ErrorMessage.ConflictPut, 409);
        }

        var exists = await parameterRepository.DoesParameterExistByParameterIdAsync(parameter);
        if (!exists)
        {
            logger.LogWarning("Parameter with ID {ParameterId} not found.", parameter.ParameterId);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        var ok = await parameterRepository.UpdateParameterAsync(parameter);
        if (!ok)
        {
            logger.LogError("Failed to update parameter with ID {ParameterId}.", parameter.ParameterId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Parameter with ID {ParameterId} updated successfully.", parameter.ParameterId);
        return Error.SetSuccess();
    }
}
