using Microsoft.AspNetCore.Mvc;
using Moq;
using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.UI.Controllers;

namespace Laboratoire.Tests.Controllers
{
    public class ProtocolControllerTests
    {
        private readonly Mock<IProtocolAdderService> _protocolAdderServiceMock;
        private readonly Mock<IProtocolGetterToDisplayService> _protocolGetterServiceMock;
        private readonly Mock<IProtocolYearGetterService> _protocolYearGetterServiceMock;
        private readonly Mock<IProtocolUpdatableService> _protocolUpdatableServiceMock;
        private readonly Mock<IProtocolPatchCashFlowIdService> _protocolPatchCashFlowIdServiceMock;
        private readonly Mock<IProtocolSpotSaverService> _protocolSpotSaverServiceMock;
        private readonly ProtocolController _controller;

        public ProtocolControllerTests()
        {
            _protocolAdderServiceMock = new Mock<IProtocolAdderService>();
            _protocolGetterServiceMock = new Mock<IProtocolGetterToDisplayService>();
            _protocolYearGetterServiceMock = new Mock<IProtocolYearGetterService>();
            _protocolUpdatableServiceMock = new Mock<IProtocolUpdatableService>();
            _protocolPatchCashFlowIdServiceMock = new Mock<IProtocolPatchCashFlowIdService>();
            _protocolSpotSaverServiceMock = new Mock<IProtocolSpotSaverService>();
            _controller = new ProtocolController(
                _protocolAdderServiceMock.Object,
                _protocolGetterServiceMock.Object,
                _protocolYearGetterServiceMock.Object,
                _protocolUpdatableServiceMock.Object,
                _protocolPatchCashFlowIdServiceMock.Object,
                _protocolSpotSaverServiceMock.Object
            );
        }

        [Fact]
        public async Task GetAllProtocolsAsync_ReturnsOk_WithProtocols()
        {
            // Arrange
            var protocols = new List<ProtocolDtoDisplay>
            {
                new ProtocolDtoDisplay { ProtocolId = "1234/2023" },
                new ProtocolDtoDisplay { ProtocolId = "5678/2023" }
            };
            _protocolGetterServiceMock.Setup(service => service.GetDisplayProtocolsAsync(2023, null, null)).ReturnsAsync(protocols);

            // Act
            var result = await _controller.GetDisplayProtocolsAsync(2024);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<ProtocolDtoDisplay>>>(okResult.Value);
            Assert.Null(response.Error);
            Assert.Equal(protocols.Count, response.Data?.Count());
        }

        [Fact]
        public async Task GetAllProtocolsAsync_ShouldReturnServerError_WhenServerErrorOccurs()
        {
            // Arrange
            _protocolGetterServiceMock.Setup(service => service.GetDisplayProtocolsAsync(2023, null, null)).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetDisplayProtocolsAsync(2023);

            // Assert
            var resultObject = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, resultObject.StatusCode);
        }

        [Fact]
        public async Task AddProtocolAsync_ReturnsConflict_WhenProtocolIsAlreadyInDatabase()
        {
            // Arrange
            var protocolDto = new ProtocolDtoAdd();
            var addResult = Error.SetError(ErrorMessage.ConflictPost, 409);
            _protocolAdderServiceMock.Setup(service => service.AddProtocolAsync(It.IsAny<ProtocolDtoAdd>())).ReturnsAsync(addResult);

            // Act
            var result = await _controller.AddProtocolAsync(protocolDto);

            // Assert
            var conflictResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(conflictResult.Value);
            Assert.Equal(409, conflictResult.StatusCode);
            Assert.Equal(ErrorMessage.ConflictPost, response?.Error?.Message);
            Assert.Null(response?.Data);
        }
        [Fact]
        public async Task AddProtocolAsync_ReturnsCreated_WhenProtocolIsAddedSuccessfully()
        {
            // Arrange
            var protocolDto = new ProtocolDtoAdd();
            var addResult = Error.SetSuccess();
            _protocolAdderServiceMock.Setup(service => service.AddProtocolAsync(It.IsAny<ProtocolDtoAdd>())).ReturnsAsync(addResult);

            // Act
            var result = await _controller.AddProtocolAsync(protocolDto);

            // Assert
            var createdResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(createdResult.Value);
            Assert.Equal(201, createdResult.StatusCode);
            Assert.Equal(SuccessMessage.Added, response.Data);
            Assert.Null(response.Error);
        }

        [Fact]
        public async Task AddProtocolAsync_ShouldReturnServerError_WhenServerErrorOccurs()
        {
            // Arrange
            var protocolDto = new ProtocolDtoAdd();
            _protocolAdderServiceMock.Setup(service => service.AddProtocolAsync(It.IsAny<ProtocolDtoAdd>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.AddProtocolAsync(protocolDto);

            // Assert
            var resultObject = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, resultObject.StatusCode);
        }

        [Fact]
        public async Task UpdateProtocolAsync_ReturnsOk_WhenProtocolIsUpdatedSuccessfully()
        {
            // Arrange
            var protocolId = "1234/2023";
            var protocolDto = new ProtocolDtoUpdate { ProtocolId = protocolId };
            var updateResult = Error.SetSuccess();
            _protocolUpdatableServiceMock.Setup(service => service.UpdateProtocolAsync(It.IsAny<ProtocolDtoUpdate>())).ReturnsAsync(updateResult);

            // Act
            var result = await _controller.UpdateProtocolAsync("1234", "2023", protocolDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal(SuccessMessage.Updated, response.Data);
            Assert.Null(response.Error);
        }

        [Fact]
        public async Task UpdateProtocolAsync_ReturnsBadRequest_WhenProtocolIdsDiffer()
        {
            // Arrange
            var protocolDto = new ProtocolDtoUpdate { ProtocolId = "5678/2023" };

            // Act
            var result = await _controller.UpdateProtocolAsync("1234", "2023", protocolDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal(400, response.Error?.Code);
            Assert.Equal(ErrorMessage.BadRequestID, response.Error?.Message);
        }

        [Fact]
        public async Task UpdateProtocolAsync_ShouldReturnNotFound_WhenProtocolIsNotOnDatabase()
        {
            // Arrange
            var protocolDto = new ProtocolDtoUpdate { ProtocolId = "1234/2023" };
            var addError = Error.SetError(ErrorMessage.NotFound, 404);
            _protocolUpdatableServiceMock.Setup(service => service.UpdateProtocolAsync(It.IsAny<ProtocolDtoUpdate>())).ReturnsAsync(addError);

            // Act
            var result = await _controller.UpdateProtocolAsync("1234", "2023", protocolDto);

            // Assert
            var resultObject = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(resultObject.Value);
            Assert.Equal(404, resultObject.StatusCode);
            Assert.Equal(ErrorMessage.NotFound, response.Error?.Message);
            Assert.Null(response?.Data);
        }

        [Fact]
        public async Task UpdateProtocolAsync_ShouldReturnServerError_WhenServerErrorOccurs()
        {
            // Arrange
            var protocolDto = new ProtocolDtoUpdate { ProtocolId = "1234/2023" };
            _protocolUpdatableServiceMock.Setup(service => service.UpdateProtocolAsync(It.IsAny<ProtocolDtoUpdate>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.UpdateProtocolAsync("1234", "2023", protocolDto);

            // Assert
            var resultObject = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, resultObject.StatusCode);
        }
    }
}
