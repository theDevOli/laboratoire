using Laboratoire.Application.Services.PropertyServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.PropertyServices
{
    public class PropertyUpdatableServiceTest
    {
        private readonly Mock<IPropertyRepository> _repositoryMock;
        private readonly Mock<ILogger<PropertyUpdatableService>> _loggerMock;
        private readonly PropertyUpdatableService _service;

        public PropertyUpdatableServiceTest()
        {
            _repositoryMock = new Mock<IPropertyRepository>();
            _loggerMock = new Mock<ILogger<PropertyUpdatableService>>();
            _service = new PropertyUpdatableService(
                _repositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task UpdatePropertyAsync_ShouldReturnNotFound_WhenPropertyDoesNotExist()
        {
            // Arrange
            var property = new Property { PropertyId = 1, PropertyName = "Test House" };
            _repositoryMock.Setup(r => r.GetPropertyByIdAsync(property.PropertyId))
                           .ReturnsAsync((Property?)null);

            // Act
            var result = await _service.UpdatePropertyAsync(property);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(404, result.StatusCode);
            _repositoryMock.Verify(r => r.GetPropertyByIdAsync(property.PropertyId), Times.Once);
            _repositoryMock.Verify(r => r.UpdatePropertyAsync(It.IsAny<Property>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePropertyAsync_ShouldReturnDbError_WhenUpdateFails()
        {
            // Arrange
            var property = new Property { PropertyId = 1, PropertyName = "Test House" };
            _repositoryMock.Setup(r => r.GetPropertyByIdAsync(property.PropertyId))
                           .ReturnsAsync(property);
            _repositoryMock.Setup(r => r.UpdatePropertyAsync(property))
                           .ReturnsAsync(false);

            // Act
            var result = await _service.UpdatePropertyAsync(property);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            _repositoryMock.Verify(r => r.GetPropertyByIdAsync(property.PropertyId), Times.Once);
            _repositoryMock.Verify(r => r.UpdatePropertyAsync(property), Times.Once);
        }

        [Fact]
        public async Task UpdatePropertyAsync_ShouldReturnSuccess_WhenUpdateSucceeds()
        {
            // Arrange
            var property = new Property { PropertyId = 1, PropertyName = "Test House" };
            _repositoryMock.Setup(r => r.GetPropertyByIdAsync(property.PropertyId))
                           .ReturnsAsync(property);
            _repositoryMock.Setup(r => r.UpdatePropertyAsync(property))
                           .ReturnsAsync(true);

            // Act
            var result = await _service.UpdatePropertyAsync(property);

            // Assert
            Assert.False(result.IsNotSuccess());
            Assert.Equal(0, result.StatusCode);
            _repositoryMock.Verify(r => r.GetPropertyByIdAsync(property.PropertyId), Times.Once);
            _repositoryMock.Verify(r => r.UpdatePropertyAsync(property), Times.Once);
        }
    }
}
