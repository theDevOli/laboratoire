using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.ChemicalServices;

public class ChemicalGetterService
(
    IChemicalRepository chemicalRepository,
    IChemicalsNormalizationRepository chemicalsNormalizationRepository,
    ILogger<ChemicalGetterService> logger
)
: IChemicalGetterService
{    public async Task<IEnumerable<ChemicalDtoGetUpdate>> GetAllChemicalsAsync()
    {
        logger.LogInformation("Starting retrieval of all chemicals.");
        var chemicals = await chemicalRepository.GetAllChemicalsAsync();
        var hazards = await chemicalsNormalizationRepository.GetAllHazardsAsync();
        return chemicals.Select(
            chemical =>
            {
                var tempHazards = hazards
                .Where(hazard => hazard.ChemicalId == chemical.ChemicalId)
                .Select(hazard => hazard.HazardId)
                .ToArray();
                return chemical.ToChemicalNormalization(hazards);
            }
        );
    }
}
