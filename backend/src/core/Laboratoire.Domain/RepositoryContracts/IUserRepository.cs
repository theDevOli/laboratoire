using Laboratoire.Domain.Entity;

namespace Laboratoire.Domain.RepositoryContracts;

public interface IUserRepository
{
    Task<IEnumerable<DisplayUser>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(Guid? userId);
    Task<Authentication?> GetAuthenticationByIdAsync(Guid? userId);
    Task<User?> GetUserByUsernameAsync(string? username);
    Task<bool> DoesUserExistByIdAsync(User user);
    Task<bool> DoesUserExistByUsernameAsync(User user);
    Task<Guid?> AddUserAsync(User user);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> UpdateUserStatusAsync(User user);
    Task<bool> UserRenameAsync(User user);
    Task<string> SetUserNameAsync(string? username);

}
