using System;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.UtilServices;

public class StateGetterService
(
    IUtilsRepository utilsRepository,
    ILogger<StateGetterService> logger
)
: IStateGetterService
{
    public Task<IEnumerable<State>?> GetAllStatesAsync()
    {
        logger.LogInformation("Fetching all states.");

        return utilsRepository.GetAllStatesAsync();
    }
}