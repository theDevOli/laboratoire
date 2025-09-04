using Laboratoire.Application.Services.HazardServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.HazardServices
{
    public class HazardGetterByIdServiceTest
    {
        private readonly Mock<IHazardRepository> _repositoryMock;
        private readonly Mock<ILogger<HazardGetterByIdService>> _loggerMock;
        private readonly HazardGetterByIdService _service;

        public HazardGetterByIdServiceTest()
        {
            _repositoryMock = new Mock<IHazardRepository>();
            _loggerMock = new Mock<ILogger<HazardGetterByIdService>>();
            _service = new HazardGetterByIdService(
                _repositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetHazardByIdAsync_ShouldReturnNull_WhenHazardIdIsNull()
        {
            // Act
            var result = await _service.GetHazardByIdAsync(null);

            // Assert
            Assert.Null(result);
            _repositoryMock.Verify(r => r.GetHazardByIdAsync(It.IsAny<int?>()), Times.Never);
        }

        [Fact]
        public async Task GetHazardByIdAsync_ShouldReturnHazard_WhenHazardExists()
        {
            // Arrange
            var hazard = new Hazard { HazardId = 1, HazardClass = "Flammable" };

            _repositoryMock.Setup(r => r.GetHazardByIdAsync(1))
                           .ReturnsAsync(hazard);

            // Act
            var result = await _service.GetHazardByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.HazardId);
            Assert.Equal("Flammable", result.HazardClass);
            _repositoryMock.Verify(r => r.GetHazardByIdAsync(It.IsAny<int?>()), Times.Once);
        }

        [Fact]
        public async Task GetHazardByIdAsync_ShouldReturnNull_WhenHazardDoesNotExist()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetHazardByIdAsync(2))
                           .ReturnsAsync((Hazard?)null);

            // Act
            var result = await _service.GetHazardByIdAsync(2);

            // Assert
            Assert.Null(result);
            _repositoryMock.Verify(r => r.GetHazardByIdAsync(It.IsAny<int?>()), Times.Once);
        }
    }
}
