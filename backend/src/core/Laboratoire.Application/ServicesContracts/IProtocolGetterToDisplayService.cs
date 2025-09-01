using Laboratoire.Application.DTO;

namespace Laboratoire.Application.ServicesContracts;

public interface IProtocolGetterToDisplayService
{
    Task<IEnumerable<ProtocolDtoDisplay>?> GetDisplayProtocolsAsync(int year, Guid? partnerId = null, bool? isPartner = null);
}
