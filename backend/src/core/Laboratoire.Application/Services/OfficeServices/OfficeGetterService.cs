using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.OfficeServices;

public class OfficeGetterService(IOfficeRepository officeRepository, ILogger<OfficeGetterService> logger) : IOfficeGetterService
{
    public Task<IEnumerable<Office>> GetAllOfficesAsync()
    {
        logger.LogInformation("Fetching all offices from the repository.");
        
        return officeRepository.GetAllOfficesAsync();
    }
}
