using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.ChemicalServices;

public class ChemicalsNormalizationGetterService
(
    IChemicalsNormalizationRepository chemicalsNormalizationRepository,
    ILogger<ChemicalsNormalizationGetterService> logger
)
: IChemicalsNormalizationGetterService
{

    public  Task<IEnumerable<ChemicalsNormalization>> GetAllHazardsAsync()
    {
        logger.LogInformation("Fetching all chemical hazard relations.");
        return chemicalsNormalizationRepository.GetAllHazardsAsync();
    }
}
