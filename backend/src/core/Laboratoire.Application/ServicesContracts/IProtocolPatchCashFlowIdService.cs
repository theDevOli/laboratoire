using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface IProtocolPatchCashFlowIdService
{
    Task<Error> PatchCashFlowIdAsync(ProtocolDtoUpdateCashFlow protocolDto);
}
