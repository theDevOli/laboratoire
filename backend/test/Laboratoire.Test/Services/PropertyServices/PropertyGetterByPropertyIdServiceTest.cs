using Laboratoire.Application.Services.PropertyServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.PropertyServices
{
    public class PropertyGetterByPropertyIdServiceTest
    {
        private readonly Mock<IPropertyRepository> _repositoryMock;
        private readonly Mock<ILogger<PropertyGetterByPropertyIdService>> _loggerMock;
        private readonly PropertyGetterByPropertyIdService _service;

        public PropertyGetterByPropertyIdServiceTest()
        {
            _repositoryMock = new Mock<IPropertyRepository>();
            _loggerMock = new Mock<ILogger<PropertyGetterByPropertyIdService>>();
            _service = new PropertyGetterByPropertyIdService(
                _repositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetPropertyByPropertyIdAsync_ShouldReturnNull_WhenPropertyIdIsNull()
        {
            // Act
            var result = await _service.GetPropertyByPropertyIdAsync(null);

            // Assert
            Assert.Null(result);
            _repositoryMock.Verify(r => r.GetPropertyByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetPropertyByPropertyIdAsync_ShouldReturnProperty_WhenPropertyExists()
        {
            // Arrange
            var propertyId = 1;
            var property = new Property { PropertyId = propertyId, PropertyName= "Test Property" };

            _repositoryMock.Setup(r => r.GetPropertyByIdAsync(propertyId))
                           .ReturnsAsync(property);

            // Act
            var result = await _service.GetPropertyByPropertyIdAsync(propertyId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(property.PropertyId, result!.PropertyId);
            _repositoryMock.Verify(r => r.GetPropertyByIdAsync(propertyId), Times.Once);
        }

        [Fact]
        public async Task GetPropertyByPropertyIdAsync_ShouldReturnNull_WhenNoPropertyExist()
        {
            // Arrange
            var propertyId = 1;
            var property = new Property { PropertyId = 2, PropertyName= "Test Property" };

            _repositoryMock.Setup(r => r.GetPropertyByIdAsync(propertyId))
                           .ReturnsAsync((Property?)null);

            // Act
            var result = await _service.GetPropertyByPropertyIdAsync(propertyId);

            // Assert
            Assert.Null(result);
            _repositoryMock.Verify(r => r.GetPropertyByIdAsync(propertyId), Times.Once);
        }
    }
}
