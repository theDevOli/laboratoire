using Laboratoire.Domain.Entity;

namespace Laboratoire.Domain.RepositoryContracts;

public interface IParameterRepository
{
    Task<IEnumerable<Parameter>> GetAllParametersAsync();
    Task<Parameter?> GetParameterByParameterIdAsync(int? parameterId);
     Task<IEnumerable<Parameter>?> GetParametersByReportIdAsync(Guid? reportId);
    Task<IEnumerable<T>?> GetParametersInputByCatalogIdAsync<T>(int? catalogId);
    Task<Parameter?> GetUniqueParameterAsync(Parameter parameter);
    Task<bool> DoesParameterExistByParameterIdAsync(Parameter parameter);
    Task<bool> IsParameterUniqueAsync(Parameter parameter);
    Task<bool> AddParameterAsync(Parameter parameter);
    Task<bool> UpdateParameterAsync(Parameter parameter);
}
