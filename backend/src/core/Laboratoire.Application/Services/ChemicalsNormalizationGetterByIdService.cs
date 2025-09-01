using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class ChemicalsNormalizationGetterByIdService
(
    IChemicalsNormalizationRepository chemicalsNormalizationRepository,
    ILogger<ChemicalsNormalizationGetterByIdService> logger
)
: IChemicalsNormalizationGetterByIdService
{
    public async Task<IEnumerable<ChemicalsNormalization>?> GetHazardsByIdAsync(int? chemicalId)
    {
        if (chemicalId is null)
        {
            logger.LogWarning("GetHazardsByIdAsync called with null chemicalId.");
            return null;
        }
        
        return await chemicalsNormalizationRepository.GetHazardsByIdAsync(chemicalId);
    }
}
