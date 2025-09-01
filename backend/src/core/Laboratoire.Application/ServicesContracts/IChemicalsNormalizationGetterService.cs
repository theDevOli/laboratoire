using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IChemicalsNormalizationGetterService
{
    Task<IEnumerable<ChemicalsNormalization>> GetAllHazardsAsync();
}
