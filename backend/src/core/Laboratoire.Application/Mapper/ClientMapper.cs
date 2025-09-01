using Laboratoire.Application.DTO;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.Mapper;

public static class ClientMapper
{
    public static Client ToClient(this ClientDtoAdd dto)
    {
        return new Client()
        {
            ClientId = default,
            ClientName = dto.ClientName?.Trim(),
            ClientTaxId = dto.ClientTaxId?.Trim(),
            ClientEmail = dto.ClientEmail?.Trim(),
            ClientPhone = dto.ClientPhone?.Trim()

        };
    }

    public static UserDtoAdd ToUser(this ClientDtoAdd dto)
=> new UserDtoAdd()
{
    RoleId = 5,
    Username = dto.ClientTaxId?.Trim(),
    IsActive = true,
};

}
