using Laboratoire.Domain.Entity;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface IClientUpdatableService
{
    Task<Error> UpdateClientAsync(Client client);
}
