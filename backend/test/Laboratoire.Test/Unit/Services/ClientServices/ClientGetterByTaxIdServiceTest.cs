using Laboratoire.Application.Services.ClientServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ClientServices
{
    public class ClientGetterByTaxIdServiceTest
    {
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly Mock<ILogger<ClientGetterByTaxIdService>> _loggerMock;
        private readonly ClientGetterByTaxIdService _service;

        public ClientGetterByTaxIdServiceTest()
        {
            _clientRepositoryMock = new Mock<IClientRepository>();
            _loggerMock = new Mock<ILogger<ClientGetterByTaxIdService>>();
            _service = new ClientGetterByTaxIdService(_clientRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetClientByTaxIdAsync_ShouldReturnNull_WhenClientTaxIdIsNull()
        {
            // Act
            var result = await _service.GetClientByTaxIdAsync(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetClientByTaxIdAsync_ShouldReturnClient_WhenClientTaxIdIsValid()
        {
            // Arrange
            var taxId = "123456789";
            var client = new Client { ClientId = Guid.NewGuid(), ClientTaxId = taxId };

            _clientRepositoryMock.Setup(r => r.GetByTaxIdAsync(taxId))
                                 .ReturnsAsync(client);

            // Act
            var result = await _service.GetClientByTaxIdAsync(taxId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taxId, result!.ClientTaxId);

            _clientRepositoryMock.Verify(r => r.GetByTaxIdAsync(taxId), Times.Once);
        }
    }
}
