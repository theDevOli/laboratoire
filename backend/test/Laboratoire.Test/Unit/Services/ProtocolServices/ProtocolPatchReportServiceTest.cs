using Laboratoire.Application.Services.ProtocolServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ProtocolServices
{
    public class ProtocolPatchReportServiceTest
    {
        private readonly Mock<IProtocolRepository> _protocolRepoMock;
        private readonly Mock<ILogger<ProtocolPatchReportService>> _loggerMock;
        private readonly ProtocolPatchReportService _service;

        public ProtocolPatchReportServiceTest()
        {
            _protocolRepoMock = new Mock<IProtocolRepository>();
            _loggerMock = new Mock<ILogger<ProtocolPatchReportService>>();
            _service = new ProtocolPatchReportService(_protocolRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task PatchReportIdAsync_ShouldReturnNotFound_WhenProtocolDoesNotExist()
        {
            var reportPatch = new ReportPatch { ProtocolId = "0001/2025", ReportId = Guid.NewGuid() };

            _protocolRepoMock.Setup(r => r.DoesProtocolExistByProtocolIdAsync(It.IsAny<string>()))
                             .ReturnsAsync(false);

            var result = await _service.PatchReportIdAsync(reportPatch);

            Assert.True(result.IsNotSuccess());
            Assert.Equal(404, result.StatusCode);
            Assert.Equal(ErrorMessage.NotFound, result.Message);
            _protocolRepoMock.Verify(r => r.DoesProtocolExistByProtocolIdAsync(It.IsAny<string>()), Times.Once);
            _protocolRepoMock.Verify(r => r.PatchReportIdAsync(It.IsAny<ReportPatch>()), Times.Never);
        }

        [Fact]
        public async Task PatchReportIdAsync_ShouldReturnDbError_WhenPatchFails()
        {
            var reportPatch = new ReportPatch { ProtocolId = "0001/2025", ReportId = Guid.NewGuid() };

            _protocolRepoMock.Setup(r => r.DoesProtocolExistByProtocolIdAsync(It.IsAny<string>()))
                             .ReturnsAsync(true);
            _protocolRepoMock.Setup(r => r.PatchReportIdAsync(It.IsAny<ReportPatch>()))
                             .ReturnsAsync(false);

            var result = await _service.PatchReportIdAsync(reportPatch);

            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            Assert.Equal(ErrorMessage.DbError, result.Message);
            _protocolRepoMock.Verify(r => r.DoesProtocolExistByProtocolIdAsync(It.IsAny<string>()), Times.Once);
            _protocolRepoMock.Verify(r => r.PatchReportIdAsync(It.IsAny<ReportPatch>()), Times.Once);
        }

        [Fact]
        public async Task PatchReportIdAsync_ShouldReturnSuccess_WhenPatchSucceeds()
        {
            var reportPatch = new ReportPatch { ProtocolId = "0001/2025", ReportId = Guid.NewGuid() };

            _protocolRepoMock.Setup(r => r.DoesProtocolExistByProtocolIdAsync(It.IsAny<string>()))
                             .ReturnsAsync(true);
            _protocolRepoMock.Setup(r => r.PatchReportIdAsync(It.IsAny<ReportPatch>()))
                             .ReturnsAsync(true);

            var result = await _service.PatchReportIdAsync(reportPatch);

            Assert.False(result.IsNotSuccess());
            Assert.Equal(0, result.StatusCode);
            Assert.Null(result.Message);
            _protocolRepoMock.Verify(r => r.DoesProtocolExistByProtocolIdAsync(It.IsAny<string>()), Times.Once);
            _protocolRepoMock.Verify(r => r.PatchReportIdAsync(It.IsAny<ReportPatch>()), Times.Once);
        }
    }
}
