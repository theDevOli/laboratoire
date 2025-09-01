using Laboratoire.Domain.Entity;

namespace Laboratoire.Domain.RepositoryContracts;

public interface IChemicalsNormalizationRepository
{
    Task<IEnumerable<ChemicalsNormalization>> GetAllHazardsAsync();
    Task<IEnumerable<ChemicalsNormalization>?> GetHazardsByIdAsync(int? chemicalId);
    Task<int?> CountHazardAsync(int? chemicalId);
    Task<bool> AddHazardAsync(IEnumerable<ChemicalsNormalization> hazardsNormalization);
    Task<bool> DeleteHazardAsync(int? chemicalId);
}
