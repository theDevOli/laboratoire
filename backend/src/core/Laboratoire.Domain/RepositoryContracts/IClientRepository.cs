
using Laboratoire.Domain.Entity;

namespace Laboratoire.Domain.RepositoryContracts;

public interface IClientRepository
{
    Task<IEnumerable<Client>> GetAllClientsAsync(string? filter);
    Task<IEnumerable<Client>> GetClientsLikeAsync(string? taxId);
    Task<Client?> GetByClientIdAsync(Guid? clientId);
    Task<Client?> GetByTaxIdAsync(string? taxId);

    Task<bool> DoesClientExistByClientIdAsync(Client client);
    Task<bool> DoesClientExistByTaxIdAsync(Client client);
    Task<bool> AddClientAsync(Client client, Guid? userId);
    Task<bool> UpdateClientAsync(Client client);
}
