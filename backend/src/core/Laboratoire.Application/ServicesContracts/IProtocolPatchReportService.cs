using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IProtocolPatchReportService
{
    Task<Error> PatchReportIdAsync(ReportPatch reportPatch);
}
