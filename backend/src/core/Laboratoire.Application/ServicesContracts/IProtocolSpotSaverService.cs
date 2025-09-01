using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface IProtocolSpotSaverService
{
    Task<Error> SaveProtocolSpotAsync(int? quantity);
}
