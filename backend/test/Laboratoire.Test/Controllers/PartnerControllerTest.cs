using Microsoft.AspNetCore.Mvc;
using Moq;

using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.UI.Controllers;

namespace Laboratoire.Tests.Controllers
{
    public class PartnerControllerTests
    {
        private readonly Mock<IPartnerGetterService> _partnerGetterServiceMock;
        private readonly Mock<IPartnerGetterByIdService> _partnerGetterByIdServiceMock;
        private readonly Mock<IPartnerAdderService> _partnerAdderServiceMock;
        private readonly Mock<IPartnerUpdatableService> _partnerUpdatableServiceMock;
        private readonly PartnerController _controller;

        public PartnerControllerTests()
        {
            _partnerGetterServiceMock = new Mock<IPartnerGetterService>();
            _partnerGetterByIdServiceMock = new Mock<IPartnerGetterByIdService>();
            _partnerAdderServiceMock = new Mock<IPartnerAdderService>();
            _partnerUpdatableServiceMock = new Mock<IPartnerUpdatableService>();
            _controller = new PartnerController(
                _partnerGetterServiceMock.Object,
                _partnerGetterByIdServiceMock.Object,
                _partnerAdderServiceMock.Object,
                _partnerUpdatableServiceMock.Object
            );
        }

        [Fact]
        public async Task GetAllPartnersAsync_ReturnsOk_WithPartners()
        {
            // Arrange
            var partners = new List<Partner>
            {
                new Partner { PartnerId = Guid.NewGuid(), PartnerName = "Partner1" },
                new Partner { PartnerId = Guid.NewGuid(), PartnerName = "Partner2" }
            };
            _partnerGetterServiceMock.Setup(service => service.GetAllPartnersAsync()).ReturnsAsync(partners);

            // Act
            var result = await _controller.GetAllPartnersAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<Partner>>>(okResult.Value);
            Assert.Null(response.Error);
            Assert.Equal(partners.Count, response.Data?.Count());
        }

        [Fact]
        public async Task GetAllPartnersAsync_ShouldReturnServerError_WhenServerErrorOccurs()
        {
            // Arrange
            _partnerGetterServiceMock.Setup(service => service.GetAllPartnersAsync()).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetAllPartnersAsync();

            // Assert
            var resultObject = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, resultObject.StatusCode);
        }

        [Fact]
        public async Task GetPartnerByIdAsync_ReturnsPartner_WhenPartnerExists()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = new Partner { PartnerId = partnerId, PartnerName = "Partner1" };
            _partnerGetterByIdServiceMock.Setup(service => service.GetPartnerByIdAsync(It.IsAny<Guid>())).ReturnsAsync(partner);

            // Act
            var result = await _controller.GetPartnerByIdAsync(partnerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<Partner>>(okResult.Value);
            Assert.Null(response.Error);
            Assert.Equal(partner.PartnerId, response.Data?.PartnerId);
        }

        [Fact]
        public async Task GetPartnerByIdAsync_ShouldReturnNotFound_WhenPartnerDoesNotExist()
        {
            // Arrange
            _partnerGetterByIdServiceMock.Setup(service => service.GetPartnerByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Partner)null!);

            // Act
            var result = await _controller.GetPartnerByIdAsync(Guid.NewGuid());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal(404, response.Error?.Code);
            Assert.Null(response.Data);
        }

        [Fact]
        public async Task GetPartnerByIdAsync_ShouldReturnServerError_WhenServerErrorOccurs()
        {
            // Arrange
            _partnerGetterByIdServiceMock.Setup(service => service.GetPartnerByIdAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetPartnerByIdAsync(new Guid());

            // Assert
            var resultObject = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, resultObject.StatusCode);
        }

        [Fact]
        public async Task AddPartnerAsync_ReturnsOk_WhenPartnerIsAddedSuccessfully()
        {
            // Arrange
            var partnerDto = new PartnerDtoAdd { PartnerName = "Partner1" };
            var addResult = Error.SetSuccess();
            _partnerAdderServiceMock.Setup(service => service.AddPartnerAsync(It.IsAny<PartnerDtoAdd>())).ReturnsAsync(addResult);

            // Act
            var result = await _controller.AddPartnerAsync(partnerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal(SuccessMessage.Added, response.Data);
            Assert.Null(response.Error);
        }

        [Fact]
        public async Task AddPartnerAsync_ReturnsConflict_WhenPartnerIsAlreadyRegistered()
        {
            // Arrange
            var partnerDto = new PartnerDtoAdd { PartnerName = "Partner1" };
            var addResult = Error.SetError(ErrorMessage.ConflictPost, 409);
            _partnerAdderServiceMock.Setup(service => service.AddPartnerAsync(It.IsAny<PartnerDtoAdd>())).ReturnsAsync(addResult);

            // Act
            var result = await _controller.AddPartnerAsync(partnerDto);

            // Assert
            var conflictResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(conflictResult.Value);
            Assert.Equal(409, response.Error?.Code);
            Assert.Equal(ErrorMessage.ConflictPost, response.Error?.Message);
            Assert.Null(response.Data);
        }

        [Fact]
        public async Task AddPartnerAsync_ShouldReturnServerError_WhenServerErrorOccurs()
        {
            // Arrange
            var partnerDto = new PartnerDtoAdd { PartnerName = "Partner1" };
            _partnerAdderServiceMock.Setup(service => service.AddPartnerAsync(It.IsAny<PartnerDtoAdd>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.AddPartnerAsync(partnerDto);

            // Assert
            var resultObject = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, resultObject.StatusCode);
        }

        [Fact]
        public async Task UpdatePartnerAsync_ReturnsOk_WhenPartnerIsUpdatedSuccessfully()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = new Partner { PartnerId = partnerId, PartnerName = "UpdatedPartner" };
            var updateResult = Error.SetSuccess();
            _partnerUpdatableServiceMock.Setup(service => service.UpdatePartnerAsync(It.IsAny<Partner>())).ReturnsAsync(updateResult);

            // Act
            var result = await _controller.UpdatePartnerAsync(partnerId, partner);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Null(response.Error);
            Assert.Equal(SuccessMessage.Updated, response.Data);
        }

        [Fact]
        public async Task UpdatePartnerAsync_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            // Arrange
            var partner = new Partner { PartnerId = Guid.NewGuid(), PartnerName = "Partner1" };

            // Act
            var result = await _controller.UpdatePartnerAsync(Guid.NewGuid(), partner);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal(ErrorMessage.BadRequestID, response.Error?.Message);
            Assert.Equal(400, response.Error?.Code);
        }

        [Fact]
        public async Task UpdatePartnerAsync_ReturnsNotFound_WhenThereIsNoRecordInDatabase()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = new Partner { PartnerId = partnerId, PartnerName = "UpdatedPartner" };
            var updateResult = Error.SetError(ErrorMessage.NotFound, 404);
            _partnerUpdatableServiceMock.Setup(service => service.UpdatePartnerAsync(It.IsAny<Partner>())).ReturnsAsync(updateResult);

            // Act
            var result = await _controller.UpdatePartnerAsync(partnerId, partner);

            // Assert
            var notFoundResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal(ErrorMessage.NotFound, response.Error?.Message);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task UpdatePartnerAsync_ShouldReturnServerError_WhenServerErrorOccurs()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = new Partner { PartnerId = partnerId, PartnerName = "UpdatedPartner" };
            _partnerUpdatableServiceMock.Setup(service => service.UpdatePartnerAsync(It.IsAny<Partner>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.UpdatePartnerAsync(partnerId, partner);

            // Assert
            var resultObject = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, resultObject.StatusCode);
        }
    }
}
