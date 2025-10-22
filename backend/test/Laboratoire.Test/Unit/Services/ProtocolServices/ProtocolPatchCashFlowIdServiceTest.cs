using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;
using Laboratoire.Application.Services.ProtocolServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Unit.Services.ProtocolServices;

public class ProtocolPatchCashFlowIdServiceTest
{
    private readonly Mock<IProtocolRepository> _protocolRepoMock;
    private readonly Mock<ILogger<ProtocolPatchCashFlowIdService>> _loggerMock;
    private readonly ProtocolPatchCashFlowIdService _service;

    public ProtocolPatchCashFlowIdServiceTest()
    {
        _protocolRepoMock = new Mock<IProtocolRepository>();
        _loggerMock = new Mock<ILogger<ProtocolPatchCashFlowIdService>>();

        _service = new ProtocolPatchCashFlowIdService(
            _protocolRepoMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task PatchCashFlowIdAsync_ShouldReturnNotFound_WhenProtocolDoesNotExist()
    {
        var dto = new ProtocolDtoUpdateCashFlow { ProtocolId = "0001/2025", Description = "Test" };

        _protocolRepoMock.Setup(r => r.DoesProtocolExistByProtocolIdAsync(It.IsAny<Protocol>()))
                         .ReturnsAsync(false);

        var result = await _service.PatchCashFlowIdAsync(dto);

        Assert.True(result.IsNotSuccess());
        Assert.Equal(404, result.StatusCode);
        Assert.Equal(ErrorMessage.NotFound, result.Message);
        _protocolRepoMock.Verify(r => r.DoesProtocolExistByProtocolIdAsync(It.IsAny<Protocol>()), Times.Once);
        _protocolRepoMock.Verify(r => r.PatchReportAsync(It.IsAny<Protocol>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task PatchCashFlowIdAsync_ShouldReturnDbError_WhenPatchReportAsyncFails()
    {
        var description = "test";
        var dto = new ProtocolDtoUpdateCashFlow { ProtocolId = "0001/2025", Description = description };

        _protocolRepoMock.Setup(r => r.DoesProtocolExistByProtocolIdAsync(It.IsAny<Protocol>()))
                         .ReturnsAsync(true);
        _protocolRepoMock.Setup(r => r.PatchReportAsync(It.IsAny<Protocol>(), It.IsAny<string>()))
                         .ReturnsAsync(false);

        var result = await _service.PatchCashFlowIdAsync(dto);

        Assert.True(result.IsNotSuccess());
        Assert.Equal(500, result.StatusCode);
        Assert.Equal(ErrorMessage.DbError, result.Message);
        _protocolRepoMock.Verify(r => r.DoesProtocolExistByProtocolIdAsync(It.IsAny<Protocol>()), Times.Once);
        _protocolRepoMock.Verify(r => r.PatchReportAsync(It.IsAny<Protocol>(), It.IsAny<string>()), Times.Once);
    }
    
    [Fact]
    public async Task PatchCashFlowIdAsync_ShouldReturnSuccess_WhenEverythingSucceeds()
    {
        var dto = new ProtocolDtoUpdateCashFlow { ProtocolId = "0001/2025", CashFlowId = 1, Description = "Test" };

        _protocolRepoMock.Setup(r => r.DoesProtocolExistByProtocolIdAsync(It.IsAny<Protocol>()))
                         .ReturnsAsync(true);
        _protocolRepoMock.Setup(r => r.PatchReportAsync(It.IsAny<Protocol>(),It.IsAny<string>()))
                         .ReturnsAsync(true);

        var result = await _service.PatchCashFlowIdAsync(dto);

        Assert.False(result.IsNotSuccess());
        Assert.Equal(0, result.StatusCode);
        Assert.Null(result.Message);
        _protocolRepoMock.Verify(r => r.DoesProtocolExistByProtocolIdAsync(It.IsAny<Protocol>()), Times.Once);
        _protocolRepoMock.Verify(r => r.PatchReportAsync(It.IsAny<Protocol>(),It.IsAny<string>()), Times.Once);
    }
}