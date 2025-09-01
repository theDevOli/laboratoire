using System.Security.Authentication;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Domain.RepositoryContracts;

public interface IUtilsRepository
{
    Task<IEnumerable<State>?> GetAllStatesAsync();
    Task<string?> GetPostalCodeByCityAndStateAsync(string? city, int? stateId);
}
