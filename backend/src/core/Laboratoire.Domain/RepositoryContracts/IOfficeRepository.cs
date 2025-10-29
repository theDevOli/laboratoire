using Laboratoire.Domain.Entity;

namespace Laboratoire.Domain.RepositoryContracts;

public interface IOfficeRepository
{
    Task<IEnumerable<Office>> GetAllOfficesAsync();
    Task<Office?> GetOfficeByIdAsync(Guid? officeId);
    Task<bool> DoesOfficeExistByIdAsync(Office office);
    Task<bool> DoesOfficeExistByCityAndNameAsync(Office office);
    Task<bool> AddOfficeAsync(Office office);
    Task<bool> UpdateOfficeAsync(Office office);
}
