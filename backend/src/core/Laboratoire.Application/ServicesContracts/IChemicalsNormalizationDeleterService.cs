using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface IChemicalsNormalizationDeleterService
{
    Task<Error> DeleteHazardAsync(int? chemicalId);
}
