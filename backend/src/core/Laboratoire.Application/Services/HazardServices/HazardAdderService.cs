using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.HazardServices;

public class HazardAdderService
(
    IHazardRepository hazardRepository,
    ILogger<HazardAdderService> logger
)
: IHazardAdderService
{
    public async Task<Error> AddHazardAsync(HazardDtoAdd hazardDto)
    {
        logger.LogInformation("Starting to add a new hazard with class: {HazardClass}", hazardDto.HazardClass);
        var hazard = hazardDto.ToHazard();

        var exists = await hazardRepository.DoesHazardExistByClassAsync(hazard);
        if (exists)
        {
            logger.LogWarning("Hazard with class '{HazardClass}' already exists.", hazard.HazardClass);
            return Error.SetError(ErrorMessage.ConflictPost, 409);
        }

        var ok = await hazardRepository.AddHazardAsync(hazard);
        if (!ok)
        {
            logger.LogError("Failed to insert hazard with class: {HazardClass}", hazard.HazardClass);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Hazard with class '{HazardClass}' added successfully.", hazard.HazardClass);

        return Error.SetSuccess();
    }
}
