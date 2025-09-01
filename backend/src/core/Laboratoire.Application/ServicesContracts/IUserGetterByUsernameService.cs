using System;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IUserGetterByUsernameService
{
    Task<User?> GetUserByUsernameAsync(string? username);
}
