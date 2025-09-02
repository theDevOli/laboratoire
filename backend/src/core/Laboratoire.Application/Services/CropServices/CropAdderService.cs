using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.CropServices;

public class CropAdderService
(
    ICropRepository cropRepository,
    ILogger<CropAdderService> logger
)
: ICropAdderService
{
    public async Task<Error> AddCropAsync(CropDtoAdd cropDto)
    {
        logger.LogInformation("Starting crop addition process for: {CropName}", cropDto.CropName);

        var crop = cropDto.ToCrop();

        var exist = await cropRepository.DoesCropExistByNameAsync(crop);
        if (exist)
        {
            logger.LogWarning("Crop with name '{CropName}' already exists.", crop.CropName);
            return Error.SetError(ErrorMessage.ConflictPost, 409);
        }

        var ok = await cropRepository.AddCropAsync(crop);
        if (!ok)
        {
            logger.LogError("Failed to add crop '{CropName}' to the database.", crop.CropName);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Crop '{CropName}' added successfully.", crop.CropName);

        return Error.SetSuccess();
    }
}
