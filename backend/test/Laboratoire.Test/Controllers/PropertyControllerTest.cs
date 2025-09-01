using Microsoft.AspNetCore.Mvc;
using Moq;

using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.UI.Controllers;

namespace Laboratoire.Tests.Controllers
{
    public class PropertyControllerTests
    {
        private readonly Mock<IPropertyGetterService> _propertyGetterServiceMock;
        private readonly Mock<IPropertyGetterByPropertyIdService> _propertyGetterByPropertyIdServiceMock;
        private readonly Mock<IPropertyGetterByClientIdService> _propertyGetterByClientIdServiceMock;
        private readonly Mock<IPropertyGetterToDisplayService> _propertyGetterToDisplayServiceMock;
        private readonly Mock<IPropertyAdderService> _propertyAdderServiceMock;
        private readonly Mock<IPropertyUpdatableService> _propertyUpdatableServiceMock;
        private readonly PropertyController _controller;

        public PropertyControllerTests()
        {
            _propertyGetterServiceMock = new Mock<IPropertyGetterService>();
            _propertyGetterByPropertyIdServiceMock = new Mock<IPropertyGetterByPropertyIdService>();
            _propertyGetterByClientIdServiceMock = new Mock<IPropertyGetterByClientIdService>();
            _propertyGetterToDisplayServiceMock = new Mock<IPropertyGetterToDisplayService>();
            _propertyAdderServiceMock = new Mock<IPropertyAdderService>();
            _propertyUpdatableServiceMock = new Mock<IPropertyUpdatableService>();
            _controller = new PropertyController(
                _propertyGetterServiceMock.Object,
                _propertyGetterByPropertyIdServiceMock.Object,
                _propertyGetterByClientIdServiceMock.Object,
                _propertyGetterToDisplayServiceMock.Object,
                _propertyAdderServiceMock.Object,
                _propertyUpdatableServiceMock.Object
            );
        }

        [Fact]
        public async Task GetAllPropertiesAsync_ReturnsOk_WithProperties()
        {
            // Arrange
            var properties = new List<Property>
            {
                new Property { PropertyId = 1, PropertyName = "Property1" },
                new Property { PropertyId = 2, PropertyName = "Property2" }
            };
            _propertyGetterServiceMock.Setup(service => service.GetAllPropertiesAsync()).ReturnsAsync(properties);

            // Act
            var result = await _controller.GetAllPropertiesAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<Property>>>(okResult.Value);
            Assert.Null(response.Error);
            Assert.Equal(properties.Count, response.Data?.Count());
        }

        [Fact]
        public async Task GetAllPropertiesAsync_ShouldReturnServerError_WhenServerErrorOccurs()
        {
            // Arrange
            _propertyGetterServiceMock.Setup(service => service.GetAllPropertiesAsync()).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetAllPropertiesAsync();

            // Assert
            var resultObject = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, resultObject.StatusCode);
        }

        [Fact]
        public async Task GetPropertyByIdAsync_ReturnsProperty_WhenPropertyExists()
        {
            // Arrange
            var propertyId = 1;
            var property = new Property { PropertyId = propertyId, PropertyName = "Property1" };
            _propertyGetterByPropertyIdServiceMock.Setup(service => service.GetPropertyByPropertyIdAsync(It.IsAny<int>())).ReturnsAsync(property);

            // Act
            var result = await _controller.GetPropertyByIdAsync(propertyId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<Property>>(okResult.Value);
            Assert.Null(response.Error);
            Assert.Equal(property.PropertyId, response.Data?.PropertyId);
        }

        [Fact]
        public async Task GetPropertyByIdAsync_ShouldReturnNotFound_WhenPropertyDoesNotExist()
        {
            // Arrange
            _propertyGetterByPropertyIdServiceMock.Setup(service => service.GetPropertyByPropertyIdAsync(It.IsAny<int>())).ReturnsAsync((Property)null!);

            // Act
            var result = await _controller.GetPropertyByIdAsync(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal(404, response.Error?.Code);
            Assert.Null(response.Data);
        }

        [Fact]
        public async Task GetPropertyByIdAsync_ShouldReturnServerError_WhenServerErrorOccurs()
        {
            // Arrange
            _propertyGetterByPropertyIdServiceMock.Setup(service => service.GetPropertyByPropertyIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetPropertyByIdAsync(1);

            // Assert
            var resultObject = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, resultObject.StatusCode);
        }

        [Fact]
        public async Task AddPropertyAsync_ReturnsOk_WhenPropertyIsAddedSuccessfully()
        {
            // Arrange
            var propertyDto = new PropertyDtoAdd { PropertyName = "Property1" };
            var addResult = Error.SetSuccess();
            _propertyAdderServiceMock.Setup(service => service.AddPropertyAsync(It.IsAny<PropertyDtoAdd>())).ReturnsAsync(addResult);

            // Act
            var result = await _controller.AddPropertyAsync(propertyDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal(SuccessMessage.Added, response.Data);
            Assert.Null(response.Error);
        }

        [Fact]
        public async Task AddPropertyAsync_ShouldReturnServerError_WhenServerErrorOccurs()
        {
            // Arrange
            var propertyDto = new PropertyDtoAdd { PropertyName = "Property1" };
            _propertyAdderServiceMock.Setup(service => service.AddPropertyAsync(It.IsAny<PropertyDtoAdd>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.AddPropertyAsync(propertyDto);

            // Assert
            var resultObject = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, resultObject.StatusCode);
        }

        [Fact]
        public async Task UpdatePropertyAsync_ReturnsOk_WhenPropertyIsUpdatedSuccessfully()
        {
            // Arrange
            var propertyId = 1;
            var property = new Property { PropertyId = propertyId, PropertyName = "UpdatedProperty" };
            var updateResult = Error.SetSuccess();
            _propertyUpdatableServiceMock.Setup(service => service.UpdatePropertyAsync(It.IsAny<Property>())).ReturnsAsync(updateResult);

            // Act
            var result = await _controller.UpdatePropertyAsync(propertyId, property);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Null(response.Error);
            Assert.Equal(SuccessMessage.Updated, response.Data);
        }

        [Fact]
        public async Task UpdatePropertyAsync_ReturnsNotFound_WhenPropertyIsFoundOnTheDatabase()
        {
            // Arrange
            var propertyId = 1;
            var property = new Property { PropertyId = propertyId, PropertyName = "UpdatedProperty" };
            var updateResult = Error.SetError(ErrorMessage.NotFound, 404);
            _propertyUpdatableServiceMock.Setup(service => service.UpdatePropertyAsync(It.IsAny<Property>())).ReturnsAsync(updateResult);

            // Act
            var result = await _controller.UpdatePropertyAsync(propertyId, property);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(objectResult.Value);
            Assert.Null(response.Data);
            Assert.Equal(ErrorMessage.NotFound, response.Error?.Message);
            Assert.Equal(404, response.Error?.Code);
        }

        [Fact]
        public async Task UpdatePropertyAsync_ReturnsBadRequest_WhenPropertyIdsDiffers()
        {
            // Arrange
            var propertyId = 1;
            var property = new Property { PropertyId = 2, PropertyName = "UpdatedProperty" };

            // Act
            var result = await _controller.UpdatePropertyAsync(propertyId, property);

            // Assert
            var objectResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(objectResult.Value);
            Assert.Null(response.Data);
            // Assert.Equal(ErrorMessage.BadRequest, response.Error?.Message);
            Assert.Equal(400, response.Error?.Code);
        }

        [Fact]
        public async Task UpdatePropertyAsync_ShouldReturnServerError_WhenServerErrorOccurs()
        {
            // Arrange
            var propertyId = 1;
            var property = new Property { PropertyId = propertyId, PropertyName = "UpdatedProperty" };
            _propertyUpdatableServiceMock.Setup(service => service.UpdatePropertyAsync(It.IsAny<Property>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.UpdatePropertyAsync(propertyId, property);

            // Assert
            var resultObject = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, resultObject.StatusCode);
        }
    }
}
