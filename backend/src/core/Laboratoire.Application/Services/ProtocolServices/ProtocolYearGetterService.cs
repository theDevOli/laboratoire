using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.ProtocolServices;

public class ProtocolYearGetterService
(
    IProtocolRepository protocolRepository,
    ILogger<ProtocolYearGetterService> logger
)
: IProtocolYearGetterService
{
    public Task<IEnumerable<ProtocolDtoYears>> GetProtocolYearsAsync()
    {
        logger.LogInformation("Fetching all distinct protocol years.");

        return protocolRepository.GetProtocolYearsAsync<ProtocolDtoYears>();
    }
}
