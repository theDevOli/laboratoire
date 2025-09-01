using Laboratoire.Application.DTO;

namespace Laboratoire.Application.ServicesContracts;

public interface IParameterInputGetterService
{
Task<IEnumerable<ParameterDtoDisplay>?> GetParameterInputByIdAsync(int? catalogId);
}
