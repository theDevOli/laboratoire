using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class FertilizerGetterService
(
    IFertilizerRepository fertilizerRepository,
    ILogger<FertilizerGetterService> logger
) : IFertilizerGetterService
{

    public async Task<IEnumerable<FertilizerDtoGet>> GetAllFertilizersAsync()
    {
        logger.LogInformation("Fetching all fertilizers from repository.");
        var fertilizers = await fertilizerRepository.GetAllFertilizersAsync();
        return fertilizers.ToDto();
    }
}
