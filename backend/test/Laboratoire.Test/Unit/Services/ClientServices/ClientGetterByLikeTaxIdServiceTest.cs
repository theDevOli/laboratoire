using Laboratoire.Application.Services.ClientServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;


namespace Laboratoire.Test.Services.ClientServices
{
    public class ClientGetterByLikeTaxIdServiceTest
    {
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly Mock<ILogger<ClientGetterByLikeTaxIdService>> _loggerMock;
        private readonly ClientGetterByLikeTaxIdService _service;

        public ClientGetterByLikeTaxIdServiceTest()
        {
            _clientRepositoryMock = new Mock<IClientRepository>();
            _loggerMock = new Mock<ILogger<ClientGetterByLikeTaxIdService>>();
            _service = new ClientGetterByLikeTaxIdService(_clientRepositoryMock.Object, _loggerMock.Object);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetClientsByLikeTaxId_ShouldReturnEmpty_WhenTaxIdIsNullOrEmpty(string taxId)
        {
            // Act
            var result = await _service.GetClientsByLikeTaxId(taxId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetClientsByLikeTaxId_ShouldReturnClients_WhenTaxIdIsValid()
        {
            // Arrange
            var taxId = "123";
            var clients = new List<Client>
            {
                new Client { ClientId = Guid.NewGuid(), ClientTaxId = "12345" },
                new Client { ClientId = Guid.NewGuid(), ClientTaxId = "12367" }
            };

            _clientRepositoryMock.Setup(r => r.GetClientsLikeAsync(taxId))
                                 .ReturnsAsync(clients);

            // Act
            var result = await _service.GetClientsByLikeTaxId(taxId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<Client>)result).Count);

            _clientRepositoryMock.Verify(r => r.GetClientsLikeAsync(taxId), Times.Once);
        }
    }
}
