using Laboratoire.Application.DTO;

namespace Laboratoire.Application.ServicesContracts;

public interface IProtocolYearGetterService
{
    Task<IEnumerable<ProtocolDtoYears>> GetProtocolYearsAsync();
}
