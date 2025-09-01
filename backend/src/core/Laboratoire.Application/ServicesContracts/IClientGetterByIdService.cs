using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IClientGetterByIdService
{
    Task<Client?> GetClientByIdAsync(Guid? clientId);
}
