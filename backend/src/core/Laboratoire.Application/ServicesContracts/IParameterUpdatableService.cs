using Laboratoire.Domain.Entity;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface IParameterUpdatableService
{
    Task<Error> UpdateParameterAsync(Parameter parameter);
}
