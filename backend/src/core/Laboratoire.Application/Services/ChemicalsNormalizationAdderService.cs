using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class ChemicalsNormalizationAdderService
(
    IChemicalsNormalizationRepository chemicalsNormalizationRepository,
    IChemicalsNormalizationDeleterService chemicalsNormalizationDeleterService,
    ILogger<ChemicalsNormalizationAdderService> logger
)
: IChemicalsNormalizationAdderService
{
    public async Task<Error> AddHazardAsync(IEnumerable<ChemicalsNormalization> hazardsNormalization)
    {
        logger.LogInformation("Starting AddHazardAsync process.");
        var chemicalId = hazardsNormalization.FirstOrDefault()?.ChemicalId;
        if (chemicalId is null)
        {
            logger.LogWarning("ChemicalId is null in the first element of the hazardsNormalization list.");
            return Error.SetError(ErrorMessage.BadRequestFirstIdNull, 400);
        }

        var isBadRequestId = hazardsNormalization.Where(hazard => hazard.HazardId is null)?.Count() > 0;
        if (isBadRequestId)
        {
            logger.LogWarning("One or more hazard entries contain null HazardId for chemical ID {ChemicalId}.", chemicalId);
            return Error.SetError(ErrorMessage.BadRequestIdNotNull, 400);
        }

        logger.LogInformation("Deleting existing hazards for chemical ID {ChemicalId}.", chemicalId);
        var deleteError = await chemicalsNormalizationDeleterService.DeleteHazardAsync(chemicalId);
        if (deleteError.IsNotSuccess())
        {
            logger.LogError("Failed to delete existing hazards for chemical ID {ChemicalId}. Error: {ErrorMessage}", chemicalId, deleteError.Message);
            return deleteError;
        }

        var ok = await chemicalsNormalizationRepository.AddHazardAsync(hazardsNormalization);

        if (!ok)
        {
            logger.LogError("Failed to add hazard relations for chemical ID {ChemicalId}.", chemicalId);
            return Error.SetError(ErrorMessage.IDOutRange, 400);
        }

        return Error.SetSuccess();
    }
}
