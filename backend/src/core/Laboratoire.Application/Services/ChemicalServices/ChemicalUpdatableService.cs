using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Application.Utils;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.ChemicalServices;

public class ChemicalUpdatableService
(
    IChemicalRepository chemicalRepository,
    IChemicalsNormalizationAdderService chemicalNormalizationAdIChemicalsNormalizationAdderService,
    ILogger<ChemicalUpdatableService> logger
)
: IChemicalUpdatableService
{
    public async Task<Error> UpdateChemicalAsync(ChemicalDtoGetUpdate chemicalDto)
    {
        var chemical = chemicalDto.ToChemical();

        var isConflict = await chemicalRepository.DoesChemicalExistByNameAndConcentrationAsync(chemical);
        if (isConflict)
        {
            logger.LogWarning("Conflict: Another chemical already exists with the same name and concentration. ID: {ChemicalId}", chemical.ChemicalId);
            return Error.SetError(ErrorMessage.ConflictPost, 409);
        }

        var exists = await chemicalRepository.DoesChemicalExistByIdAsync(chemical);
        if (!exists)
        {
            logger.LogWarning("Chemical with ID {ChemicalId} not found.", chemical.ChemicalId);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        var ok = await chemicalRepository.UpdateChemicalAsync(chemical);
        if (!ok)
        {
            logger.LogError("Failed to update chemical with ID {ChemicalId}.", chemical.ChemicalId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        var hazards = chemicalDto.Hazards?.Select(hazard => new ChemicalsNormalization
        {
            ChemicalId = chemical.ChemicalId,
            HazardId = hazard
        });

        if (hazards is null)
        {
            logger.LogInformation("No hazard data provided for chemical ID: {ChemicalId}. Skipping hazard update.", chemical.ChemicalId);
            return Error.SetSuccess();
        }

        var addError = await chemicalNormalizationAdIChemicalsNormalizationAdderService.AddHazardAsync(hazards);
        if (addError.IsNotSuccess())
        {
            logger.LogError("Failed to update hazards for chemical ID: {ChemicalId}. Error: {ErrorMessage}", chemical.ChemicalId, addError.Message);
            return addError;
        }

        return Error.SetSuccess();
    }
}
