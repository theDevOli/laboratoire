using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IClientGetterByTaxIdService
{
    Task<Client?> GetClientByTaxIdAsync(string? clientTaxId);
}
