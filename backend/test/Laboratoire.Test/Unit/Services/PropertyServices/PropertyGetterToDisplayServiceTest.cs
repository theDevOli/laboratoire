using Laboratoire.Application.DTO;
using Laboratoire.Application.Services.PropertyServices;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.PropertyServices
{
    public class PropertyGetterToDisplayServiceTest
    {
        private readonly Mock<IPropertyRepository> _repositoryMock;
        private readonly Mock<ILogger<PropertyGetterToDisplayService>> _loggerMock;
        private readonly PropertyGetterToDisplayService _service;

        public PropertyGetterToDisplayServiceTest()
        {
            _repositoryMock = new Mock<IPropertyRepository>();
            _loggerMock = new Mock<ILogger<PropertyGetterToDisplayService>>();
            _service = new PropertyGetterToDisplayService(
                _repositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetAllPropertiesDisplayAsync_ShouldReturnAllPropertiesToDisplay_WhenPropertyToDisplayExists()
        {
            // Arrange
            var properties = new List<PropertyDtoDisplay>
            {
                new PropertyDtoDisplay { PropertyId = 1, PropertyName = "House 1" },
                new PropertyDtoDisplay { PropertyId = 1, PropertyName = "House 2" }
            };

            _repositoryMock.Setup(r => r.GetAllPropertiesDisplayAsync<PropertyDtoDisplay>())
                           .ReturnsAsync(properties);

            // Act
            var result = await _service.GetAllPropertiesDisplayAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Collection
            (
                result,
                item => Assert.Equal(properties[0].PropertyId, item.PropertyId),
                item => Assert.Equal(properties[1].PropertyId, item.PropertyId)
            );
            _repositoryMock.Verify(r => r.GetAllPropertiesDisplayAsync<PropertyDtoDisplay>(), Times.Once);
        }

        [Fact]
        public async Task GetAllPropertiesDisplayAsync_ShouldReturnEmptyCollection_WhenNoPropertyToDisplayExists()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllPropertiesDisplayAsync<PropertyDtoDisplay>())
                           .ReturnsAsync(Enumerable.Empty<PropertyDtoDisplay>());

            // Act
            var result = await _service.GetAllPropertiesDisplayAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _repositoryMock.Verify(r => r.GetAllPropertiesDisplayAsync<PropertyDtoDisplay>(), Times.Once);
        }
    }
}
