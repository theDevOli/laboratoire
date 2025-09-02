using Laboratoire.Domain.Entity;

namespace Laboratoire.Domain.RepositoryContracts;

public interface IProtocolRepository
{
    Task<IEnumerable<Protocol>> GetAllProtocolsAsync();
    //FIXME:Remove the filter
    // Task<IEnumerable<T>> GetDisplayProtocolsAsync<T>(int year);
    Task<IEnumerable<T>> GetDisplayProtocolsAsync<T>(int year,bool isEmployee);
    Task<IEnumerable<T>> GetProtocolYearsAsync<T>();
    Task<Protocol?> GetProtocolByProtocolIdAsync(string? protocolId);
    Task<bool> DoesProtocolExistByReportIdAsync(Guid? reportId);
    Task<Protocol?> GetProtocolByReportIdAsync(Guid? reportId);
    Task<Protocol?> GetUniqueProtocolAsync(Protocol protocol);
    Task<bool> DoesProtocolExistByUniqueAsync(Protocol protocol);
    Task<bool> DoesProtocolExistByProtocolIdAsync(Protocol protocol);
    Task<bool> DoesProtocolExistByProtocolIdAsync(string? protocolId);
    Task<bool> IsProtocolDoubled(string? protocolId);
    Task<string?> AddProtocolAsync(Protocol protocol);
    Task<bool> SaveProtocolSpotAsync(int? quantity, Guid? clientId, int protocolId);
    Task<bool> UpdateProtocolAsync(Protocol protocol);
    Task<bool> UpdateCatalogAsync(Protocol protocol);
    Task<bool> UpdateCashFlowIdAsync(Protocol protocol);
    Task<bool> PatchReportIdAsync(ReportPatch reportPatch);
}
