using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.CropServices;

public class CropsNormalizationDeleterService
(
    ICropsNormalizationRepository cropsNormalizationRepository,
    ILogger<CropsNormalizationDeleterService> logger
)
: ICropsNormalizationDeleterService
{
    public async Task<Error> DeleteCropsAsync(string? protocolId)
    {
        if (string.IsNullOrWhiteSpace(protocolId))
        {
            logger.LogWarning("Attempted to delete crops with null or empty protocolId.");
            return Error.SetError(ErrorMessage.BadRequestIdNotNull, 400);
        }

        logger.LogInformation("Checking for existing crops linked to protocol ID: {ProtocolId}", protocolId);

        var isNoneCrops = await cropsNormalizationRepository.IsThereNoneCropsAsync(protocolId);
        if (isNoneCrops)
        {
            logger.LogInformation("No crops found to delete for protocol ID: {ProtocolId}", protocolId);
            return Error.SetSuccess();
        }

        logger.LogInformation("Attempting to delete crops for protocol ID: {ProtocolId}", protocolId);

        var ok = await cropsNormalizationRepository.DeleteCropsAsync(protocolId);
        if (!ok)
        {
            logger.LogError("Failed to delete crops for protocol ID: {ProtocolId}", protocolId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Successfully deleted crops for protocol ID: {ProtocolId}", protocolId);
        return Error.SetSuccess();
    }
}
