using Laboratoire.Application.Services.ProtocolServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ProtocolServices
{
    public class ProtocolPatchCatalogServiceTest
    {
        private readonly Mock<IProtocolRepository> _protocolRepoMock;
        private readonly Mock<IReportRepository> _reportRepoMock;
        private readonly Mock<ILogger<ProtocolPatchCatalogService>> _loggerMock;
        private readonly ProtocolPatchCatalogService _service;

        public ProtocolPatchCatalogServiceTest()
        {
            _protocolRepoMock = new Mock<IProtocolRepository>();
            _reportRepoMock = new Mock<IReportRepository>();
            _loggerMock = new Mock<ILogger<ProtocolPatchCatalogService>>();

            _service = new ProtocolPatchCatalogService(
                _protocolRepoMock.Object,
                _reportRepoMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task UpdateCatalogAsync_ShouldReturnDbError_WhenUpdateFails()
        {
            var protocol = new Protocol { ProtocolId = "0001/2025" };

            _protocolRepoMock.Setup(r => r.UpdateCatalogAsync(It.IsAny<Protocol>()))
                             .ReturnsAsync(false);

            var result = await _service.UpdateCatalogAsync(protocol);

            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            Assert.Equal(ErrorMessage.DbError, result.Message);
            _protocolRepoMock.Verify(r => r.UpdateCatalogAsync(It.IsAny<Protocol>()), Times.Once);
            _protocolRepoMock.Verify(r => r.GetProtocolByProtocolIdAsync(It.IsAny<string>()), Times.Never);
            _reportRepoMock.Verify(r => r.ResetReportAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task UpdateCatalogAsync_ShouldReturnSuccess_WhenUpdateSucceedsAndNoReport()
        {
            var protocol = new Protocol { ProtocolId = "0001/2025" };

            _protocolRepoMock.Setup(r => r.UpdateCatalogAsync(It.IsAny<Protocol>()))
                             .ReturnsAsync(true);
            _protocolRepoMock.Setup(r => r.GetProtocolByProtocolIdAsync(It.IsAny<string>()))
                             .ReturnsAsync(new Protocol { ProtocolId = protocol.ProtocolId, ReportId = null });

            var result = await _service.UpdateCatalogAsync(protocol);

            Assert.False(result.IsNotSuccess());
            Assert.Equal(0, result.StatusCode);
            Assert.Null(result.Message);
            _protocolRepoMock.Verify(r => r.UpdateCatalogAsync(It.IsAny<Protocol>()), Times.Once);
            _protocolRepoMock.Verify(r => r.GetProtocolByProtocolIdAsync(It.IsAny<string>()), Times.Once);
            _reportRepoMock.Verify(r => r.ResetReportAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task UpdateCatalogAsync_ShouldReturnDbError_WhenReportResetFails()
        {
            var protocol = new Protocol { ProtocolId = "0001/2025" };
            var reportId = Guid.NewGuid();

            _protocolRepoMock.Setup(r => r.UpdateCatalogAsync(It.IsAny<Protocol>()))
                             .ReturnsAsync(true);
            _protocolRepoMock.Setup(r => r.GetProtocolByProtocolIdAsync(It.IsAny<string>()))
                             .ReturnsAsync(new Protocol { ProtocolId = protocol.ProtocolId, ReportId = reportId });
            _reportRepoMock.Setup(r => r.ResetReportAsync(It.IsAny<Guid>()))
                           .ReturnsAsync(false);

            var result = await _service.UpdateCatalogAsync(protocol);

            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            Assert.Equal(ErrorMessage.DbError, result.Message);
            _protocolRepoMock.Verify(r => r.UpdateCatalogAsync(It.IsAny<Protocol>()), Times.Once);
            _protocolRepoMock.Verify(r => r.GetProtocolByProtocolIdAsync(It.IsAny<string>()), Times.Once);
            _reportRepoMock.Verify(r => r.ResetReportAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCatalogAsync_ShouldReturnSuccess_WhenUpdateAndReportResetSucceeds()
        {
            var protocol = new Protocol { ProtocolId = "0001/2025" };
            var reportId = Guid.NewGuid();

            _protocolRepoMock.Setup(r => r.UpdateCatalogAsync(It.IsAny<Protocol>()))
                             .ReturnsAsync(true);
            _protocolRepoMock.Setup(r => r.GetProtocolByProtocolIdAsync(It.IsAny<string>()))
                             .ReturnsAsync(new Protocol { ProtocolId = protocol.ProtocolId, ReportId = reportId });
            _reportRepoMock.Setup(r => r.ResetReportAsync(It.IsAny<Guid>()))
                           .ReturnsAsync(true);

            var result = await _service.UpdateCatalogAsync(protocol);

            Assert.False(result.IsNotSuccess());
            Assert.Equal(0, result.StatusCode);
            Assert.Null(result.Message);
            _protocolRepoMock.Verify(r => r.UpdateCatalogAsync(It.IsAny<Protocol>()), Times.Once);
            _protocolRepoMock.Verify(r => r.GetProtocolByProtocolIdAsync(It.IsAny<string>()), Times.Once);
            _reportRepoMock.Verify(r => r.ResetReportAsync(It.IsAny<Guid>()), Times.Once);
        }
    }
}
