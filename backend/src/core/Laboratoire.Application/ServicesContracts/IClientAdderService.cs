using System;
using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface IClientAdderService
{
    Task<Error> AddClientAsync(ClientDtoAdd client);
}
