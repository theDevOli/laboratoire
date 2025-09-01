using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class ChemicalGetterByIdService
(
    IChemicalRepository chemicalRepository,
    IChemicalsNormalizationGetterService chemicalsNormalizationGetterService,
    ILogger<ChemicalGetterByIdService> logger
)
: IChemicalGetterByIdService
{
    public async Task<ChemicalDtoGetUpdate?> GetChemicalByIdAsync(int? chemicalId)
    {
        if (chemicalId is null)
        {
            logger.LogWarning("GetChemicalByIdAsync called with null ID.");
            return null;
        }

        logger.LogInformation("Attempting to retrieve chemical with ID: {ChemicalId}", chemicalId);

        var chemical = await chemicalRepository.GetChemicalByIdAsync(chemicalId);
        var hazards = await chemicalsNormalizationGetterService.GetAllHazardsAsync();

        return chemical?.ToChemicalNormalization(hazards) ?? null;
    }
}
