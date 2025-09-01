
using Laboratoire.Domain.Entity;

namespace Laboratoire.Domain.RepositoryContracts;

public interface IAuthRepository
{
    Task<Auth?> GetAuthByUserIdAsync(Guid? userId);
    Task<bool> DoesAuthExistsAsync(Guid? userId);
    public Task<bool> AddAuthAsync(Auth auth);
    public Task<bool> UpdateAuthAsync(Auth auth);
}
