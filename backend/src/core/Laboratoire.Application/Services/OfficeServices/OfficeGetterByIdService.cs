using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.OfficeServices;

public class OfficeGetterByIdService(IOfficeRepository officeRepository,ILogger<OfficeGetterByIdService>logger) : IOfficeGetterByIdService
{
    public async Task<Office?> GetByOfficeIdAsync(Guid? officeId)
    {
        if (officeId is null) return null;
        logger.LogInformation("Fetching office with ID: {officeId}", officeId);
        
        return await officeRepository.GetOfficeByIdAsync(officeId);
    }
}
