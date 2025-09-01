using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IClientGetterByLikeTaxIdService
{
    Task<IEnumerable<Client>> GetClientsByLikeTaxId(string? taxId);
}
