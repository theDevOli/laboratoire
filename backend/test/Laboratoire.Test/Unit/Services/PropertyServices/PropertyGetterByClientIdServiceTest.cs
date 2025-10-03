using Laboratoire.Application.Services.PropertyServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.PropertyServices
{
    public class PropertyGetterByClientIdServiceTest
    {
        private readonly Mock<IPropertyRepository> _repositoryMock;
        private readonly Mock<ILogger<PropertyGetterByClientIdService>> _loggerMock;
        private readonly PropertyGetterByClientIdService _service;

        public PropertyGetterByClientIdServiceTest()
        {
            _repositoryMock = new Mock<IPropertyRepository>();
            _loggerMock = new Mock<ILogger<PropertyGetterByClientIdService>>();
            _service = new PropertyGetterByClientIdService(
                _repositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetAllPropertiesByClientIdAsync_ShouldReturnNull_WhenClientIdIsNull()
        {
            // Act
            var result = await _service.GetAllPropertiesByClientIdAsync(null);

            // Assert
            Assert.Null(result);
            _repositoryMock.Verify(r => r.GetAllPropertiesByClientIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetAllPropertiesByClientIdAsync_ShouldReturnProperties_WhenClientHasProperty()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var properties = new List<Property>
            {
                new Property { PropertyId = 1, PropertyName = "Property 1",ClientId=clientId },
                new Property { PropertyId = 2, PropertyName = "Property 2",ClientId=clientId }
            };
            _repositoryMock.Setup(r => r.GetAllPropertiesByClientIdAsync(clientId))
                           .ReturnsAsync(properties);

            // Act
            var result = await _service.GetAllPropertiesByClientIdAsync(clientId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Collection
            (
                result,
                item => Assert.Equal(item.PropertyId, properties[0].PropertyId),
                item => Assert.Equal(item.PropertyId, properties[1].PropertyId)
            );
            _repositoryMock.Verify(r => r.GetAllPropertiesByClientIdAsync(clientId), Times.Once);
        }
        [Fact]
        public async Task GetAllPropertiesByClientIdAsync_ShouldReturnEmptyCollection_WhenClientHasNoProperty()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var properties = new List<Property>
            {
                new Property { PropertyId = 1, PropertyName = "Property 1",ClientId=Guid.NewGuid() },
                new Property { PropertyId = 2, PropertyName = "Property 2",ClientId=Guid.NewGuid() }
            };
            _repositoryMock.Setup(r => r.GetAllPropertiesByClientIdAsync(clientId))
                           .ReturnsAsync(Enumerable.Empty<Property>());

            // Act
            var result = await _service.GetAllPropertiesByClientIdAsync(clientId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _repositoryMock.Verify(r => r.GetAllPropertiesByClientIdAsync(clientId), Times.Once);
        }
    }
}
