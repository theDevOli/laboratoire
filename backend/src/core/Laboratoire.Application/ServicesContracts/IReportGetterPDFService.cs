using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IReportGetterPDFService
{
    Task<ReportPDF?> GetReportPDFAsync(Guid? reportId);
}
