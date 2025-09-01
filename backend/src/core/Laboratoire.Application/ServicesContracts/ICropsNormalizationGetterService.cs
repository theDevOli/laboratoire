using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface ICropsNormalizationGetterService
{
    Task<IEnumerable<CropsNormalization>> GetAllCropsAsync();
}
