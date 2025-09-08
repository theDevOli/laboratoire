using Laboratoire.Application.DTO;
using Laboratoire.Application.Services.PropertyServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.PropertyServices;
    public class PropertyAdderServiceTest
    {
        private readonly Mock<IPropertyRepository> _propertyRepositoryMock;
        private readonly Mock<IUtilsRepository> _utilsRepositoryMock;
        private readonly Mock<ILogger<PropertyAdderService>> _loggerMock;
        private readonly PropertyAdderService _service;

        public PropertyAdderServiceTest()
        {
            _propertyRepositoryMock = new Mock<IPropertyRepository>();
            _utilsRepositoryMock = new Mock<IUtilsRepository>();
            _loggerMock = new Mock<ILogger<PropertyAdderService>>();
            _service = new PropertyAdderService(
                _propertyRepositoryMock.Object,
                _utilsRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task AddPropertyAsync_ShouldReturnSuccess_WhenPropertyAddedWithPostalCode()
        {
            // Arrange
            var propertyDto = new PropertyDtoAdd 
            { 
                City = "Test City", 
                StateId = 1, 
                PostalCode = "12345" 
            };

            _propertyRepositoryMock.Setup(r => r.AddPropertyAsync(It.IsAny<Property>()))
                                   .ReturnsAsync(true);

            // Act
            var result = await _service.AddPropertyAsync(propertyDto);

            // Assert
            Assert.False(result.IsNotSuccess());
            Assert.Equal(0, result.StatusCode);
            Assert.Null(result.Message);
            _propertyRepositoryMock.Verify(r => r.AddPropertyAsync(It.IsAny<Property>()), Times.Once);
            _utilsRepositoryMock.Verify(u => u.GetPostalCodeByCityAndStateAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task AddPropertyAsync_ShouldRetrievePostalCode_WhenNotProvided()
        {
            // Arrange
            var propertyDto = new PropertyDtoAdd 
            { 
                City = "Test City", 
                StateId = 1, 
                PostalCode = null 
            };
            var expectedPostalCode = "67890";

            _utilsRepositoryMock.Setup(u => u.GetPostalCodeByCityAndStateAsync(propertyDto.City, propertyDto.StateId))
                               .ReturnsAsync(expectedPostalCode);
            _propertyRepositoryMock.Setup(r => r.AddPropertyAsync(It.IsAny<Property>()))
                                   .ReturnsAsync(true);

            // Act
            var result = await _service.AddPropertyAsync(propertyDto);

            // Assert
            Assert.False(result.IsNotSuccess());
            _propertyRepositoryMock.Verify(r => r.AddPropertyAsync(It.Is<Property>(p => p.PostalCode == expectedPostalCode)), Times.Once);
            _utilsRepositoryMock.Verify(u => u.GetPostalCodeByCityAndStateAsync(propertyDto.City, propertyDto.StateId), Times.Once);
        }

        [Fact]
        public async Task AddPropertyAsync_ShouldReturnDbError_WhenPropertyInsertionFails()
        {
            // Arrange
            var propertyDto = new PropertyDtoAdd 
            { 
                City = "Test City", 
                StateId = 1, 
                PostalCode = "12345" 
            };

            _propertyRepositoryMock.Setup(r => r.AddPropertyAsync(It.IsAny<Property>()))
                                   .ReturnsAsync(false);

            // Act
            var result = await _service.AddPropertyAsync(propertyDto);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            Assert.Equal(ErrorMessage.DbError, result.Message);
            _propertyRepositoryMock.Verify(r => r.AddPropertyAsync(It.IsAny<Property>()), Times.Once);
        }

        [Fact]
        public async Task AddPropertyAsync_ShouldReturnDbError_WhenPropertyInsertionFailsAfterPostalCodeRetrieval()
        {
            // Arrange
            var propertyDto = new PropertyDtoAdd 
            { 
                City = "Test City", 
                StateId = 1, 
                PostalCode = null 
            };
            var expectedPostalCode = "67890";

            _utilsRepositoryMock.Setup(u => u.GetPostalCodeByCityAndStateAsync(propertyDto.City, propertyDto.StateId))
                               .ReturnsAsync(expectedPostalCode);
            _propertyRepositoryMock.Setup(r => r.AddPropertyAsync(It.IsAny<Property>()))
                                   .ReturnsAsync(false);

            // Act
            var result = await _service.AddPropertyAsync(propertyDto);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            Assert.Equal(ErrorMessage.DbError, result.Message);
            _propertyRepositoryMock.Verify(r => r.AddPropertyAsync(It.Is<Property>(p => p.PostalCode == expectedPostalCode)), Times.Once);
            _utilsRepositoryMock.Verify(u => u.GetPostalCodeByCityAndStateAsync(propertyDto.City, propertyDto.StateId), Times.Once);
        }
    }