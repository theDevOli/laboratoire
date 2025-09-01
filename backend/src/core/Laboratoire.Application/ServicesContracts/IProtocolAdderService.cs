using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface IProtocolAdderService
{
    Task<Error> AddProtocolAsync(ProtocolDtoAdd protocol);
}
