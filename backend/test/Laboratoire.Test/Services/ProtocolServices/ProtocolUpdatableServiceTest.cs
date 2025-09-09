using Laboratoire.Application.Services.ProtocolServices;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ProtocolServices;

public class ProtocolUpdatableServiceTests
{
    private readonly Mock<IProtocolRepository> _protocolRepoMock;
    private readonly Mock<IProtocolPatchCatalogService> _catalogServiceMock;
    private readonly Mock<IReportAdderService> _reportAdderMock;
    private readonly Mock<IReportPatchService> _reportPatchMock;
    private readonly Mock<ICashFlowUpdatableService> _cashFlowUpdateMock;
    private readonly Mock<ICropsNormalizationAdderService> _cropsAdderMock;
    private readonly Mock<ICashFlowAdderService> _cashFlowAdderMock;
    private readonly Mock<ILogger<ProtocolUpdatableService>> _loggerMock;

    private readonly ProtocolUpdatableService _service;

    public ProtocolUpdatableServiceTests()
    {
        _protocolRepoMock = new Mock<IProtocolRepository>();
        _catalogServiceMock = new Mock<IProtocolPatchCatalogService>();
        _reportAdderMock = new Mock<IReportAdderService>();
        _reportPatchMock = new Mock<IReportPatchService>();
        _cashFlowUpdateMock = new Mock<ICashFlowUpdatableService>();
        _cropsAdderMock = new Mock<ICropsNormalizationAdderService>();
        _cashFlowAdderMock = new Mock<ICashFlowAdderService>();
        _loggerMock = new Mock<ILogger<ProtocolUpdatableService>>();

        _service = new ProtocolUpdatableService(
            _protocolRepoMock.Object,
            _catalogServiceMock.Object,
            _reportAdderMock.Object,
            _reportPatchMock.Object,
            _cashFlowUpdateMock.Object,
            _cropsAdderMock.Object,
            _cashFlowAdderMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task UpdateProtocolAsync_ShouldReturnNotFound_WhenProtocolDoesNotExist()
    {
        var dto = new ProtocolDtoUpdate { ProtocolId = "0001/2025" };
        _protocolRepoMock.Setup(r => r.GetProtocolByProtocolIdAsync(It.IsAny<string>()))
                         .ReturnsAsync((Protocol?)null);

        var result = await _service.UpdateProtocolAsync(dto);

        Assert.True(result.IsNotSuccess());
        Assert.Equal(404, result.StatusCode);
        _protocolRepoMock.Verify(r => r.GetProtocolByProtocolIdAsync(It.IsAny<string>()), Times.Once);
        _protocolRepoMock.Verify(r => r.UpdateProtocolAsync(It.IsAny<Protocol>()), Times.Never);
        _catalogServiceMock.Verify(s => s.UpdateCatalogAsync(It.IsAny<Protocol>()), Times.Never);
        _reportAdderMock.Verify(s => s.AddReportAsync(It.IsAny<ReportDtoAdd>()), Times.Never);
        _reportPatchMock.Verify(s => s.PatchReportAsync(It.IsAny<Report>()), Times.Never);
        _cashFlowAdderMock.Verify(s => s.AddCashFlowAsync(It.IsAny<CashFlow>(), It.IsAny<Protocol>()), Times.Never);
        _cashFlowUpdateMock.Verify(s => s.UpdateCashFlowAsync(It.IsAny<CashFlow>()), Times.Never);
        _cropsAdderMock.Verify(s => s.AddCropsAsync(It.IsAny<IEnumerable<CropsNormalization>>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UpdateProtocolAsync_ShouldReturnDbError_WhenUpdateFails()
    {
        string protocolId = "0001/2025";
        int catalogId = 1;

        var protocol = new Protocol { ProtocolId = protocolId, CatalogId = catalogId, ReportId = null };
        var dto = new ProtocolDtoUpdate { ProtocolId = protocolId, CatalogId = catalogId, ReportId = null };

        _protocolRepoMock.Setup(r => r.GetProtocolByProtocolIdAsync(It.IsAny<string>()))
                         .ReturnsAsync(protocol);
        _protocolRepoMock.Setup(r => r.UpdateProtocolAsync(It.IsAny<Protocol>()))
                         .ReturnsAsync(false);

        var result = await _service.UpdateProtocolAsync(dto);

        Assert.True(result.IsNotSuccess());
        Assert.Equal(500, result.StatusCode);
        _protocolRepoMock.Verify(r => r.GetProtocolByProtocolIdAsync(It.IsAny<string>()), Times.Once);
        _protocolRepoMock.Verify(r => r.UpdateProtocolAsync(It.IsAny<Protocol>()), Times.Once);
        _catalogServiceMock.Verify(s => s.UpdateCatalogAsync(It.IsAny<Protocol>()), Times.Never);
        _reportAdderMock.Verify(s => s.AddReportAsync(It.IsAny<ReportDtoAdd>()), Times.Once);
        _reportPatchMock.Verify(s => s.PatchReportAsync(It.IsAny<Report>()), Times.Never);
        _cashFlowAdderMock.Verify(s => s.AddCashFlowAsync(It.IsAny<CashFlow>(), It.IsAny<Protocol>()), Times.Never);
        _cashFlowUpdateMock.Verify(s => s.UpdateCashFlowAsync(It.IsAny<CashFlow>()), Times.Never);
        _cropsAdderMock.Verify(s => s.AddCropsAsync(It.IsAny<IEnumerable<CropsNormalization>>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UpdateProtocolAsync_ShouldReturnSuccess_WhenProtocolExistsAndUpdateCatalogAndAddCrops()
    {
        string protocolId = "0001/2025";
        Guid reportId = Guid.NewGuid();

        var protocol = new Protocol { ProtocolId = protocolId, CatalogId = 1, ReportId = null };
        var dto = new ProtocolDtoUpdate { ProtocolId = protocolId, CatalogId = 2, ReportId = null };

        _protocolRepoMock.Setup(r => r.GetProtocolByProtocolIdAsync(It.IsAny<string>()))
                         .ReturnsAsync(protocol);
        _protocolRepoMock.Setup(r => r.UpdateProtocolAsync(It.IsAny<Protocol>()))
                         .ReturnsAsync(true);

        _catalogServiceMock.Setup(s => s.UpdateCatalogAsync(It.IsAny<Protocol>()))
                           .ReturnsAsync(Error.SetSuccess());
        _cashFlowUpdateMock.Setup(s => s.UpdateCashFlowAsync(It.IsAny<CashFlow>()))
                           .ReturnsAsync(Error.SetSuccess());
        _cropsAdderMock.Setup(s => s.AddCropsAsync(It.IsAny<IEnumerable<CropsNormalization>>(), It.IsAny<string>()))
                       .ReturnsAsync(Error.SetSuccess());

        var result = await _service.UpdateProtocolAsync(dto);

        Assert.False(result.IsNotSuccess());
        _protocolRepoMock.Verify(r => r.GetProtocolByProtocolIdAsync(It.IsAny<string>()), Times.Once);
        _protocolRepoMock.Verify(r => r.UpdateProtocolAsync(It.IsAny<Protocol>()), Times.Once);
        _catalogServiceMock.Verify(s => s.UpdateCatalogAsync(It.IsAny<Protocol>()), Times.Once);
        _reportAdderMock.Verify(s => s.AddReportAsync(It.IsAny<ReportDtoAdd>()), Times.Never);
        _reportPatchMock.Verify(s => s.PatchReportAsync(It.IsAny<Report>()), Times.Never);
        _cashFlowAdderMock.Verify(s => s.AddCashFlowAsync(It.IsAny<CashFlow>(), It.IsAny<Protocol>()), Times.Never);
        _cashFlowUpdateMock.Verify(s => s.UpdateCashFlowAsync(It.IsAny<CashFlow>()), Times.Never);
        _cropsAdderMock.Verify(s => s.AddCropsAsync(It.IsAny<IEnumerable<CropsNormalization>>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProtocolAsync_ShouldReturnSuccess_WhenProtocolExistsAndAddReport()
    {
        string protocolId = "0001/2025";
        Guid reportId = Guid.NewGuid();

        var protocol = new Protocol { ProtocolId = protocolId, CatalogId = 1, ReportId = null };
        var dto = new ProtocolDtoUpdate { ProtocolId = protocolId, CatalogId = 1, ReportId = null };

        _protocolRepoMock.Setup(r => r.GetProtocolByProtocolIdAsync(It.IsAny<string>()))
                         .ReturnsAsync(protocol);
        _protocolRepoMock.Setup(r => r.UpdateProtocolAsync(It.IsAny<Protocol>()))
                         .ReturnsAsync(true);
        _reportAdderMock.Setup(s => s.AddReportAsync(It.IsAny<ReportDtoAdd>()))
                        .ReturnsAsync(Error.SetSuccess());
        _cashFlowUpdateMock.Setup(s => s.UpdateCashFlowAsync(It.IsAny<CashFlow>()))
                           .ReturnsAsync(Error.SetSuccess());
        _cropsAdderMock.Setup(s => s.AddCropsAsync(It.IsAny<IEnumerable<CropsNormalization>>(), It.IsAny<string>()))
                       .ReturnsAsync(Error.SetSuccess());

        var result = await _service.UpdateProtocolAsync(dto);

        Assert.False(result.IsNotSuccess());
        _protocolRepoMock.Verify(r => r.GetProtocolByProtocolIdAsync(It.IsAny<string>()), Times.Once);
        _protocolRepoMock.Verify(r => r.UpdateProtocolAsync(It.IsAny<Protocol>()), Times.Once);
        _catalogServiceMock.Verify(s => s.UpdateCatalogAsync(It.IsAny<Protocol>()), Times.Never);
        _reportAdderMock.Verify(s => s.AddReportAsync(It.IsAny<ReportDtoAdd>()), Times.Once);
        _reportPatchMock.Verify(s => s.PatchReportAsync(It.IsAny<Report>()), Times.Never);
        _cashFlowAdderMock.Verify(s => s.AddCashFlowAsync(It.IsAny<CashFlow>(), It.IsAny<Protocol>()), Times.Never);
        _cashFlowUpdateMock.Verify(s => s.UpdateCashFlowAsync(It.IsAny<CashFlow>()), Times.Never);
        _cropsAdderMock.Verify(s => s.AddCropsAsync(It.IsAny<IEnumerable<CropsNormalization>>(), It.IsAny<string>()), Times.Once);
    }
}
