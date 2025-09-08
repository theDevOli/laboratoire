using Laboratoire.Application.Services.PropertyServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.PropertyServices
{
    public class PropertyGetterServiceTest
    {
        private readonly Mock<IPropertyRepository> _repositoryMock;
        private readonly Mock<ILogger<PropertyGetterService>> _loggerMock;
        private readonly PropertyGetterService _service;

        public PropertyGetterServiceTest()
        {
            _repositoryMock = new Mock<IPropertyRepository>();
            _loggerMock = new Mock<ILogger<PropertyGetterService>>();
            _service = new PropertyGetterService(
                _repositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetAllPropertiesAsync_ShouldReturnAllProperties_WhenPropertyExists()
        {
            // Arrange
            var properties = new List<Property>
            {
                new Property { PropertyId = 1, PropertyName = "Property 1" },
                new Property { PropertyId = 2, PropertyName = "Property 2" }
            };

            _repositoryMock.Setup(r => r.GetAllPropertiesAsync())
                           .ReturnsAsync(properties);

            // Act
            var result = await _service.GetAllPropertiesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Collection
                (
                    result,
                     item => Assert.Equal(item.PropertyId, properties[0].PropertyId),
                     item => Assert.Equal(item.PropertyId, properties[1].PropertyId)
                );
            _repositoryMock.Verify(r => r.GetAllPropertiesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllPropertiesAsync_ShouldReturnEmptyCollection_WhenNoPropertyExists()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllPropertiesAsync())
                           .ReturnsAsync(Enumerable.Empty<Property>());

            // Act
            var result = await _service.GetAllPropertiesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _repositoryMock.Verify(r => r.GetAllPropertiesAsync(), Times.Once);
        }
    }
}
