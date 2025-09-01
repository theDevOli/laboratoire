using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface IParameterAdderService
{
    Task<Error> AddParameterAsync(ParameterDtoAdd parameterDto);
}
