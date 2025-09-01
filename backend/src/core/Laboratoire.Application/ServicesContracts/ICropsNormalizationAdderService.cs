using Laboratoire.Domain.Entity;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface ICropsNormalizationAdderService
{
     Task<Error> AddCropsAsync(IEnumerable<CropsNormalization>? cropsNormalization,string protocolId);

}
