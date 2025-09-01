using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;
using Laboratoire.Application.Mapper;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class ChemicalAdderService
(
    IChemicalRepository chemicalRepository,
    IChemicalsNormalizationAdderService chemicalsNormalizationAdderService,
    ILogger<ChemicalAdderService> logger
)
: IChemicalAdderService
{

    public async Task<Error> AddChemicalAsync(ChemicalDtoAdd chemicalDto)
    {
        logger.LogInformation("Starting chemical addition process for: {ChemicalName}, {Concentration}",
    chemicalDto.ChemicalName, chemicalDto.Concentration);

        var chemical = chemicalDto.ToChemical();

        var exists = await chemicalRepository.DoesChemicalExistByNameAndConcentrationAsync(chemical);
        if (exists)
        {
            logger.LogWarning("Chemical already exists: {ChemicalName}, {Concentration}",
            chemical.ChemicalName, chemical.Concentration);
            return Error.SetError(ErrorMessage.ConflictPost, 409);
        }

        var chemicalId = await chemicalRepository.AddChemicalAsync(chemical);
        if (chemicalId is null)
        {
            logger.LogError("Failed to insert chemical into database: {ChemicalName}, {Concentration}",
        chemical.ChemicalName, chemical.Concentration);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        var hazards = chemicalDto.Hazards?.Select(hazard => new ChemicalsNormalization()
        {
            ChemicalId = chemicalId,
            HazardId = hazard
        });

        if (hazards is null)
        {
            logger.LogWarning("No valid hazards provided for chemical ID: {ChemicalId}", chemicalId);
            return Error.SetError("There are no such hazards on the database.", 404);
        }
        return await chemicalsNormalizationAdderService.AddHazardAsync(hazards);

    }

}
