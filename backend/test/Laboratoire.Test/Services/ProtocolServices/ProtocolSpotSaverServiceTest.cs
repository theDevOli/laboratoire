using Laboratoire.Application.Services.ProtocolServices;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ProtocolServices
{
    public class ProtocolSpotSaverServiceTest
    {
        private readonly Mock<IProtocolRepository> _protocolRepoMock;
        private readonly Mock<IClientGetterByTaxIdService> _clientGetterMock;
        private readonly Mock<ILogger<ProtocolSpotSaverService>> _loggerMock;
        private readonly ProtocolSpotSaverService _service;

        public ProtocolSpotSaverServiceTest()
        {
            _protocolRepoMock = new Mock<IProtocolRepository>();
            _clientGetterMock = new Mock<IClientGetterByTaxIdService>();
            _loggerMock = new Mock<ILogger<ProtocolSpotSaverService>>();
            _service = new ProtocolSpotSaverService(
                _protocolRepoMock.Object,
                _clientGetterMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task SaveProtocolSpotAsync_ShouldReturnNotFound_WhenClientNotFound()
        {
            _clientGetterMock.Setup(c => c.GetClientByTaxIdAsync(It.IsAny<string>()))
                             .ReturnsAsync((Client?)null);

            var result = await _service.SaveProtocolSpotAsync(5);

            Assert.True(result.IsNotSuccess());
            Assert.Equal(404, result.StatusCode);
            Assert.Equal(ErrorMessage.NotFound, result.Message);
            _clientGetterMock.Verify(c => c.GetClientByTaxIdAsync(It.IsAny<string>()), Times.Once);
            _protocolRepoMock.Verify(p => p.SaveProtocolSpotAsync(It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task SaveProtocolSpotAsync_ShouldReturnDbError_WhenSaveFails()
        {
            var client = new Client { ClientId = Guid.NewGuid() };
            _clientGetterMock.Setup(c => c.GetClientByTaxIdAsync(It.IsAny<string>()))
                             .ReturnsAsync(client);
            _protocolRepoMock.Setup(p => p.SaveProtocolSpotAsync(It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<int>()))
                             .ReturnsAsync(false);

            var result = await _service.SaveProtocolSpotAsync(5);

            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            Assert.Equal(ErrorMessage.DbError, result.Message);
            _clientGetterMock.Verify(c => c.GetClientByTaxIdAsync(It.IsAny<string>()), Times.Once);
            _protocolRepoMock.Verify(p => p.SaveProtocolSpotAsync(It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task SaveProtocolSpotAsync_ShouldReturnSuccess_WhenSaveSucceeds()
        {
            var client = new Client { ClientId = Guid.NewGuid() };
            _clientGetterMock.Setup(c => c.GetClientByTaxIdAsync(It.IsAny<string>()))
                             .ReturnsAsync(client);
            _protocolRepoMock.Setup(p => p.SaveProtocolSpotAsync(It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<int>()))
                             .ReturnsAsync(true);

            var result = await _service.SaveProtocolSpotAsync(5);

            Assert.False(result.IsNotSuccess());
            Assert.Equal(0, result.StatusCode);
            Assert.Null(result.Message);
            _clientGetterMock.Verify(c => c.GetClientByTaxIdAsync(It.IsAny<string>()), Times.Once);
            _protocolRepoMock.Verify(p => p.SaveProtocolSpotAsync(It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<int>()), Times.Once);
        }
    }
}
