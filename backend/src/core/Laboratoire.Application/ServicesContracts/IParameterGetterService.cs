
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IParameterGetterService
{
    Task<IEnumerable<Parameter>> GetAllParametersAsync();
}
