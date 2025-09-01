using Laboratoire.Domain.Entity;

namespace Laboratoire.Domain.RepositoryContracts;

public interface IReportRepository
{
    Task<IEnumerable<Report>> GetAllReportsAsync();
    Task<Report?> GetReportByIdAsync(Guid? reportId);
    Task<ReportPDF?> GetReportPDFAsync(Guid? reportId);
    Task<bool> DoesReportExistsAsync(Report report);
    Task<bool> IsProtocolDoubled(Report report);
    Task<Guid?> AddReportAsync(Report report);
    Task<bool> PatchReportAsync(Report report);
    Task<bool> ResetReportAsync(Guid? reportId);
}
