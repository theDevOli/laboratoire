using Laboratoire.Domain.Entity;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface IChemicalsNormalizationAdderService
{
    Task<Error> AddHazardAsync(IEnumerable<ChemicalsNormalization> hazardsNormalization);
}
