using Laboratoire.Application.DTO;
using Laboratoire.Application.Services.ProtocolServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ProtocolServices
{
    public class ProtocolPatchCashFlowIdServiceTest
    {
        private readonly Mock<IProtocolRepository> _protocolRepoMock;
        private readonly Mock<ICashFlowRepository> _cashFlowRepoMock;
        private readonly Mock<ILogger<ProtocolPatchCashFlowIdService>> _loggerMock;
        private readonly ProtocolPatchCashFlowIdService _service;

        public ProtocolPatchCashFlowIdServiceTest()
        {
            _protocolRepoMock = new Mock<IProtocolRepository>();
            _cashFlowRepoMock = new Mock<ICashFlowRepository>();
            _loggerMock = new Mock<ILogger<ProtocolPatchCashFlowIdService>>();

            _service = new ProtocolPatchCashFlowIdService(
                _protocolRepoMock.Object,
                _cashFlowRepoMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task PatchCashFlowIdAsync_ShouldReturnNotFound_WhenProtocolDoesNotExist()
        {
            var dto = new ProtocolDtoUpdateCashFlow { ProtocolId = "0001/2025" };

            _protocolRepoMock.Setup(r => r.DoesProtocolExistByProtocolIdAsync(It.IsAny<Protocol>()))
                             .ReturnsAsync(false);

            var result = await _service.PatchCashFlowIdAsync(dto);

            Assert.True(result.IsNotSuccess());
            Assert.Equal(404, result.StatusCode);
            Assert.Equal(ErrorMessage.NotFound, result.Message);
            _protocolRepoMock.Verify(r => r.DoesProtocolExistByProtocolIdAsync(It.IsAny<Protocol>()), Times.Once);
            _protocolRepoMock.Verify(r => r.UpdateCashFlowIdAsync(It.IsAny<Protocol>()), Times.Never);
            _cashFlowRepoMock.Verify(r => r.PatchDescriptionAsync(It.IsAny<CashFlow>()), Times.Never);
        }

        [Fact]
        public async Task PatchCashFlowIdAsync_ShouldReturnDbError_WhenProtocolUpdateFails()
        {
            var dto = new ProtocolDtoUpdateCashFlow { ProtocolId = "0001/2025" };

            _protocolRepoMock.Setup(r => r.DoesProtocolExistByProtocolIdAsync(It.IsAny<Protocol>()))
                             .ReturnsAsync(true);
            _protocolRepoMock.Setup(r => r.UpdateCashFlowIdAsync(It.IsAny<Protocol>()))
                             .ReturnsAsync(false);

            var result = await _service.PatchCashFlowIdAsync(dto);

            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            Assert.Equal(ErrorMessage.DbError, result.Message);
            _protocolRepoMock.Verify(r => r.DoesProtocolExistByProtocolIdAsync(It.IsAny<Protocol>()), Times.Once);
            _protocolRepoMock.Verify(r => r.UpdateCashFlowIdAsync(It.IsAny<Protocol>()), Times.Once);
            _cashFlowRepoMock.Verify(r => r.PatchDescriptionAsync(It.IsAny<CashFlow>()), Times.Never);
        }

        [Fact]
        public async Task PatchCashFlowIdAsync_ShouldReturnDbError_WhenCashFlowPatchFails()
        {
            var dto = new ProtocolDtoUpdateCashFlow { ProtocolId = "0001/2025", CashFlowId = 1, Description = "Test" };

            _protocolRepoMock.Setup(r => r.DoesProtocolExistByProtocolIdAsync(It.IsAny<Protocol>()))
                             .ReturnsAsync(true);
            _protocolRepoMock.Setup(r => r.UpdateCashFlowIdAsync(It.IsAny<Protocol>()))
                             .ReturnsAsync(true);
            _cashFlowRepoMock.Setup(r => r.PatchDescriptionAsync(It.IsAny<CashFlow>()))
                             .ReturnsAsync(false);

            var result = await _service.PatchCashFlowIdAsync(dto);

            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            Assert.Equal(ErrorMessage.DbError, result.Message);
            _protocolRepoMock.Verify(r => r.DoesProtocolExistByProtocolIdAsync(It.IsAny<Protocol>()), Times.Once);
            _protocolRepoMock.Verify(r => r.UpdateCashFlowIdAsync(It.IsAny<Protocol>()), Times.Once);
            _cashFlowRepoMock.Verify(r => r.PatchDescriptionAsync(It.IsAny<CashFlow>()), Times.Once);
        }

        [Fact]
        public async Task PatchCashFlowIdAsync_ShouldReturnSuccess_WhenEverythingSucceeds()
        {
            var dto = new ProtocolDtoUpdateCashFlow { ProtocolId = "0001/2025", CashFlowId = 1, Description = "Test" };

            _protocolRepoMock.Setup(r => r.DoesProtocolExistByProtocolIdAsync(It.IsAny<Protocol>()))
                             .ReturnsAsync(true);
            _protocolRepoMock.Setup(r => r.UpdateCashFlowIdAsync(It.IsAny<Protocol>()))
                             .ReturnsAsync(true);
            _cashFlowRepoMock.Setup(r => r.PatchDescriptionAsync(It.IsAny<CashFlow>()))
                             .ReturnsAsync(true);

            var result = await _service.PatchCashFlowIdAsync(dto);

            Assert.False(result.IsNotSuccess());
            Assert.Equal(0, result.StatusCode);
            Assert.Null(result.Message);
            _protocolRepoMock.Verify(r => r.DoesProtocolExistByProtocolIdAsync(It.IsAny<Protocol>()), Times.Once);
            _protocolRepoMock.Verify(r => r.UpdateCashFlowIdAsync(It.IsAny<Protocol>()), Times.Once);
            _cashFlowRepoMock.Verify(r => r.PatchDescriptionAsync(It.IsAny<CashFlow>()), Times.Once);
        }
    }
}
