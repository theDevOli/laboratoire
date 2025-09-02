using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Application.Mapper;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.ParameterServices;

public class ParameterAdderService
(
    IParameterRepository parameterRepository,
    ILogger<ParameterAdderService> logger
)
: IParameterAdderService
{
    public async Task<Error> AddParameterAsync(ParameterDtoAdd parameterDto)
    {
        logger.LogInformation("Starting to add a new parameter.");

        var parameter = parameterDto.ToParameter();

        var exists = await parameterRepository.IsParameterUniqueAsync(parameter);
        if (exists)
        {
            logger.LogWarning("Parameter already exists and conflicts with uniqueness constraints.");
            return Error.SetError(ErrorMessage.ConflictPost, 409);
        }

        var ok = await parameterRepository.AddParameterAsync(parameter);
        if (!ok)
        {
            logger.LogError("Failed to add parameter to the database.");
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Parameter added successfully.");
        return Error.SetSuccess();
    }
}
