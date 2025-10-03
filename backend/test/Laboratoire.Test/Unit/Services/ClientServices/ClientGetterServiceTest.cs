using Laboratoire.Application.Services.ClientServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ClientServices
{
    public class ClientGetterServiceTest
    {
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly Mock<ILogger<ClientGetterService>> _loggerMock;
        private readonly ClientGetterService _service;

        public ClientGetterServiceTest()
        {
            _clientRepositoryMock = new Mock<IClientRepository>();
            _loggerMock = new Mock<ILogger<ClientGetterService>>();
            _service = new ClientGetterService(_clientRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllClientsAsync_ShouldReturnClients_WhenFilterIsNull()
        {
            // Arrange
            var clients = new List<Client>
            {
                new Client { ClientId = Guid.NewGuid(), ClientTaxId = "123" },
                new Client { ClientId = Guid.NewGuid(), ClientTaxId = "456" }
            };

            _clientRepositoryMock.Setup(r => r.GetAllClientsAsync(null))
                                 .ReturnsAsync(clients);

            // Act
            var result = await _service.GetAllClientsAsync(null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<Client>)result).Count);

            _clientRepositoryMock.Verify(r => r.GetAllClientsAsync(null), Times.Once);
        }

        [Fact]
        public async Task GetAllClientsAsync_ShouldReturnClients_WhenFilterIsProvided()
        {
            // Arrange
            var filter = "123";
            var clients = new List<Client>
            {
                new Client { ClientId = Guid.NewGuid(), ClientTaxId = "123" }
            };

            _clientRepositoryMock.Setup(r => r.GetAllClientsAsync(filter))
                                 .ReturnsAsync(clients);

            // Act
            var result = await _service.GetAllClientsAsync(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);

            _clientRepositoryMock.Verify(r => r.GetAllClientsAsync(filter), Times.Once);
        }
    }
}
