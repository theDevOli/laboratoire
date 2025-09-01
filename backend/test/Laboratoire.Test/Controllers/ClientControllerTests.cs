using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.UI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Laboratoire.Tests.Controllers
{
    public class ClientControllerTests
    {
        private readonly Mock<IClientGetterService> _mockClientGetterService;
        private readonly Mock<IClientGetterByIdService> _mockClientGetterByIdService;
        private readonly Mock<IClientGetterByTaxIdService> _mockClientGetterByTaxIdService;
        private readonly Mock<IClientGetterByLikeTaxIdService> _mockClientGetterByLikeTaxIdService;
        private readonly Mock<IClientAdderService> _mockClientAdderService;
        private readonly Mock<IClientUpdatableService> _mockClientUpdatableService;
        private readonly ClientController _controller;

        public ClientControllerTests()
        {
            _mockClientGetterService = new Mock<IClientGetterService>();
            _mockClientGetterByIdService = new Mock<IClientGetterByIdService>();
            _mockClientGetterByTaxIdService = new Mock<IClientGetterByTaxIdService>();
            _mockClientGetterByLikeTaxIdService = new Mock<IClientGetterByLikeTaxIdService>();
            _mockClientAdderService = new Mock<IClientAdderService>();
            _mockClientUpdatableService = new Mock<IClientUpdatableService>();

            _controller = new ClientController(
                _mockClientGetterService.Object,
                _mockClientGetterByIdService.Object,
                _mockClientGetterByTaxIdService.Object,
                _mockClientGetterByLikeTaxIdService.Object,
                _mockClientAdderService.Object,
                _mockClientUpdatableService.Object
            );
        }

        [Fact]
        public async Task GetAllClientsAsync_ReturnsOkResult_WithListOfClients()
        {
            // Arrange
            var clients = new List<Client>
            {
                new Client { ClientId = new Guid(), ClientName = "Client 1",ClientTaxId="00011122233" },
                new Client { ClientId = new Guid(), ClientName = "Client 2",ClientTaxId="00022222233" },
            };

            _mockClientGetterService.Setup(service => service.GetAllClientsAsync(null))
                                    .ReturnsAsync(clients);

            // Act
            var result = await _controller.GetAllClientsAsync(null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<Client>>>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(clients.Count(), response.Data?.Count());
        }

        [Fact]
        public async Task GetAllClientsAsync_ShouldReturnServerError_WhenSeverErrorOccurs()
        {
            // Arrange
            _mockClientGetterService.Setup(repo => repo.GetAllClientsAsync(null)).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetAllClientsAsync(null);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

            Assert.Equal(500, response.Error?.Code);
            Assert.Null(response.Data);
        }

        [Fact]
        public async Task GetClientByIdAsync_ReturnsOkResult_WhenClientExists()
        {
            // Arrange
            var clientId = new Guid();
            var client = new Client { ClientId = clientId, ClientName = "Client 1", ClientTaxId = "00011122233" };

            _mockClientGetterByIdService.Setup(service => service.GetClientByIdAsync(clientId))
                                        .ReturnsAsync(client);

            // Act
            var result = await _controller.GetClientByIdAsync(clientId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<Client>>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(client, response.Data);
        }

        [Fact]
        public async Task GetClientByIdAsync_ReturnsNotFound_WhenClientDoesNotExist()
        {
            // Arrange
            var clientId = new Guid();
            Client? client = null;

            _mockClientGetterByIdService.Setup(service => service.GetClientByIdAsync(clientId))
                                        .ReturnsAsync(client);

            // Act
            var result = await _controller.GetClientByIdAsync(clientId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal(404, response.Error?.Code);
            Assert.Equal(ErrorMessage.NotFound, response.Error?.Message);
        }

        [Fact]
        public async Task GetClientByIdAsync_ShouldReturnServerError_WhenSeverErrorOccurs()
        {
            // Arrange
            Guid clientId = new Guid();
            _mockClientGetterByIdService.Setup(repo => repo.GetClientByIdAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetClientByIdAsync(clientId);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

            Assert.Equal(500, response.Error?.Code);
            Assert.Null(response.Data);
        }

        [Fact]
        public async Task AddClientAsync_ReturnsCreated_WhenClientIsAddedSuccessfully()
        {
            // Arrange
            var clientDto = new ClientDtoAdd { ClientName = "Client 1", ClientTaxId = "00011122233" };
            var successResponse = ApiResponse<object>.Success(SuccessMessage.Added);
            var expectedError = Error.SetSuccess();

            _mockClientAdderService.Setup(service => service.AddClientAsync(clientDto))
                                   .ReturnsAsync(expectedError);

            // Act
            var result = await _controller.AddClientAsync(clientDto);

            // Assert
            var createdResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(createdResult.Value);
            Assert.Equal(201, createdResult.StatusCode);
            Assert.Equal(SuccessMessage.Added, response.Data);
        }

        [Fact]
        public async Task AddClientAsync_ReturnsErrorConflict_WhenAddFails()
        {
            // Arrange
            var clientDto = new ClientDtoAdd { ClientName = "Client 1", ClientTaxId = "00011122233" };
            var expectedError = Error.SetError(ErrorMessage.ConflictPost, 409);

            _mockClientAdderService.Setup(service => service.AddClientAsync(clientDto))
                                   .ReturnsAsync(expectedError);

            // Act
            var result = await _controller.AddClientAsync(clientDto);

            // Assert
            var errorResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(errorResult.Value);
            Assert.Equal(409, response.Error?.Code);
            Assert.Equal(ErrorMessage.ConflictPost, response.Error?.Message);
        }

        [Fact]
        public async Task AddClientAsync_ShouldReturnServerError_WhenSeverErrorOccurs()
        {
            // Arrange
            var clientDto = new ClientDtoAdd { ClientName = "Client 1", ClientTaxId = "00011122233" };
            _mockClientAdderService.Setup(repo => repo.AddClientAsync(It.IsAny<ClientDtoAdd>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.AddClientAsync(clientDto);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

            Assert.Equal(500, response.Error?.Code);
            Assert.Null(response.Data);
        }

        [Fact]
        public async Task UpdateClientAsync_ReturnsOk_WhenClientIsUpdatedSuccessfully()
        {
            // Arrange
            var clientId = new Guid();
            var client = new Client { ClientId = clientId, ClientName = "Client 1", ClientTaxId = "00011122233" };
            var expectedError = Error.SetSuccess();

            _mockClientUpdatableService.Setup(service => service.UpdateClientAsync(client))
                                       .ReturnsAsync(expectedError);

            // Act
            var result = await _controller.UpdateClientAsync(clientId, client);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(SuccessMessage.Updated, response.Data);
        }

        [Fact]
        public async Task UpdateClientAsync_ReturnsBadRequest_WhenClientIdDoesNotMatch()
        {
            // Arrange
            var clientId = new Guid();
            var client = new Client { ClientId = new Guid(), ClientName = "Client 1", ClientTaxId = "00011122233" };

            // Act
            var result = await _controller.UpdateClientAsync(clientId, client);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal(400, response.Error?.Code);
            Assert.Equal(ErrorMessage.BadRequestID, response.Error?.Message);
        }

        [Fact]
        public async Task UpdateClientAsync_ReturnsNotFound_WhenUpdateFails()
        {
            // Arrange
            var clientId = new Guid();
            var client = new Client { ClientId = clientId, ClientName = "Client 1", ClientTaxId = "00011122233" };
            var expectedError = Error.SetError(ErrorMessage.NotFound, 404);

            _mockClientUpdatableService.Setup(service => service.UpdateClientAsync(client))
                                       .ReturnsAsync(expectedError);

            // Act
            var result = await _controller.UpdateClientAsync(clientId, client);

            // Assert
            var errorResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(errorResult.Value);
            Assert.Equal(404, response.Error?.Code);
            Assert.Equal(ErrorMessage.NotFound, response.Error?.Message);
        }

        [Fact]
        public async Task UpdateClientAsync_ShouldReturnServerError_WhenSeverErrorOccurs()
        {
            // Arrange
            Guid clientId = new Guid();
            var client = new Client { ClientId = clientId, ClientName = "Client 1", ClientTaxId = "00011122233" };
            _mockClientUpdatableService.Setup(repo => repo.UpdateClientAsync(It.IsAny<Client>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.UpdateClientAsync(clientId, client);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

            Assert.Equal(500, response.Error?.Code);
            Assert.Null(response.Data);
        }
    }
}
