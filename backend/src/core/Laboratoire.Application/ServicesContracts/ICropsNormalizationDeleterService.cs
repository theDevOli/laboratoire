using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface ICropsNormalizationDeleterService
{
    Task<Error> DeleteCropsAsync(string? protocolId);
}
