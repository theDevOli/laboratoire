using Laboratoire.Application.Services.ClientServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ClientServices
{
    public class ClientGetterByIdServiceTest
    {
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly Mock<ILogger<ClientGetterByIdService>> _loggerMock;
        private readonly ClientGetterByIdService _service;

        public ClientGetterByIdServiceTest()
        {
            _clientRepositoryMock = new Mock<IClientRepository>();
            _loggerMock = new Mock<ILogger<ClientGetterByIdService>>();
            _service = new ClientGetterByIdService(_clientRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetClientByIdAsync_ShouldReturnNull_WhenIdIsNull()
        {
            // Act
            var result = await _service.GetClientByIdAsync(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetClientByIdAsync_ShouldReturnClient_WhenClientExists()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var expectedClient = new Client { ClientId = clientId, ClientTaxId = "123456789" };

            _clientRepositoryMock.Setup(r => r.GetByClientIdAsync(clientId))
                                 .ReturnsAsync(expectedClient);

            // Act
            var result = await _service.GetClientByIdAsync(clientId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedClient.ClientId, result!.ClientId);
            Assert.Equal(expectedClient.ClientTaxId, result.ClientTaxId);

            _clientRepositoryMock.Verify(r => r.GetByClientIdAsync(clientId), Times.Once);
        }

        [Fact]
        public async Task GetClientByIdAsync_ShouldReturnNull_WhenClientDoesNotExist()
        {
            // Arrange
            var clientId = Guid.NewGuid();

            _clientRepositoryMock.Setup(r => r.GetByClientIdAsync(clientId))
                                 .ReturnsAsync((Client?)null);

            // Act
            var result = await _service.GetClientByIdAsync(clientId);

            // Assert
            Assert.Null(result);
            _clientRepositoryMock.Verify(r => r.GetByClientIdAsync(clientId), Times.Once);
        }
    }
}
