using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IParameterGetterByIdService
{
    Task<Parameter?> GetParameterByIdAsync(int? parameterId);
}
