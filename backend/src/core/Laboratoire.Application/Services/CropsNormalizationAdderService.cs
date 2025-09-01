using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class CropsNormalizationAdderService
(
    ICropsNormalizationRepository cropsNormalizationRepository,
    ICropsNormalizationDeleterService cropsNormalizationDeleterService,
    ILogger<CropsNormalizationAdderService> logger
)
: ICropsNormalizationAdderService
{
    public async Task<Error> AddCropsAsync(IEnumerable<CropsNormalization>? cropsNormalization, string protocolId)
    {
        logger.LogInformation("Starting crop normalization update for protocol ID: {ProtocolId}", protocolId);
        var deleteError = await cropsNormalizationDeleterService.DeleteCropsAsync(protocolId);
        if (deleteError.IsNotSuccess())
        {
            logger.LogWarning("Failed to delete existing crops for protocol ID: {ProtocolId}", protocolId);
            return deleteError;
        }

        if (cropsNormalization is null)
        {
            logger.LogInformation("No crops provided to add for protocol ID: {ProtocolId}. Only deletion performed.", protocolId);
            return Error.SetSuccess();
        }

        var ok = await cropsNormalizationRepository.AddCropsAsync(cropsNormalization);
        if (!ok)
        {
            logger.LogError("Failed to add crop normalizations for protocol ID: {ProtocolId}", protocolId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Successfully added crop normalizations for protocol ID: {ProtocolId}", protocolId);
        return Error.SetSuccess();
    }
}
