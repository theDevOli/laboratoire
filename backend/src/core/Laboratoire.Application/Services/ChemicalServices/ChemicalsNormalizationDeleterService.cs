
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.ChemicalServices;

public class ChemicalsNormalizationDeleterService
(
    IChemicalsNormalizationRepository chemicalsNormalizationRepository,
    ILogger<ChemicalsNormalizationDeleterService> logger
)
: IChemicalsNormalizationDeleterService
{
    public async Task<Error> DeleteHazardAsync(int? chemicalId)
    {
        if (chemicalId is null)
        {
            logger.LogWarning("DeleteHazardAsync called with null chemicalId.");
            return Error.SetError(ErrorMessage.BadRequest, 400);
        }

        var countChemicals = await chemicalsNormalizationRepository.CountHazardAsync(chemicalId);

        if (countChemicals == 0)
        {
            logger.LogInformation("No hazards to delete for chemical ID: {ChemicalId}", chemicalId);
            return Error.SetSuccess();
        }

        logger.LogInformation("Deleting {Count} hazards for chemical ID: {ChemicalId}", countChemicals, chemicalId);
        var ok = await chemicalsNormalizationRepository.DeleteHazardAsync(chemicalId);
        if (!ok)
        {
            logger.LogError("Failed to delete hazards for chemical ID: {ChemicalId}", chemicalId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }


        return Error.SetSuccess();
    }
}
