using Laboratoire.Application.Services.HazardServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.HazardServices
{
    public class HazardUpdatableServiceTest
    {
        private readonly Mock<IHazardRepository> _repositoryMock;
        private readonly Mock<ILogger<HazardUpdatableService>> _loggerMock;
        private readonly HazardUpdatableService _service;

        public HazardUpdatableServiceTest()
        {
            _repositoryMock = new Mock<IHazardRepository>();
            _loggerMock = new Mock<ILogger<HazardUpdatableService>>();
            _service = new HazardUpdatableService(
                _repositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task UpdateHazardAsync_ShouldReturnConflict_WhenHazardClassAlreadyExists()
        {
            // Arrange
            var hazard = new Hazard { HazardId = 1, HazardClass = "Flammable" };

            _repositoryMock.Setup(r => r.DoesHazardExistByClassAsync(It.IsAny<Hazard>()))
                           .ReturnsAsync(true);

            // Act
            var result = await _service.UpdateHazardAsync(hazard);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(409, result.StatusCode);
            Assert.Equal(ErrorMessage.ConflictPut, result.Message);
            _repositoryMock.Verify(r => r.DoesHazardExistByClassAsync(It.IsAny<Hazard>()), Times.Once);
            _repositoryMock.Verify(r => r.DoesHazardExistByIdAsync(It.IsAny<Hazard>()), Times.Never);
            _repositoryMock.Verify(r => r.UpdateHazardAsync(It.IsAny<Hazard>()), Times.Never);
        }

        [Fact]
        public async Task UpdateHazardAsync_ShouldReturnNotFound_WhenHazardDoesNotExist()
        {
            // Arrange
            var hazard = new Hazard { HazardId = 2, HazardClass = "Toxic" };

            _repositoryMock.Setup(r => r.DoesHazardExistByClassAsync(It.IsAny<Hazard>()))
                           .ReturnsAsync(false);
            _repositoryMock.Setup(r => r.DoesHazardExistByIdAsync(It.IsAny<Hazard>()))
                           .ReturnsAsync(false);

            // Act
            var result = await _service.UpdateHazardAsync(hazard);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(404, result.StatusCode);
            Assert.Equal(ErrorMessage.NotFound, result.Message);
            _repositoryMock.Verify(r => r.DoesHazardExistByClassAsync(It.IsAny<Hazard>()), Times.Once);
            _repositoryMock.Verify(r => r.DoesHazardExistByIdAsync(It.IsAny<Hazard>()), Times.Once);
            _repositoryMock.Verify(r => r.UpdateHazardAsync(It.IsAny<Hazard>()), Times.Never);
        }

        [Fact]
        public async Task UpdateHazardAsync_ShouldReturnDbError_WhenUpdateFails()
        {
            // Arrange
            var hazard = new Hazard { HazardId = 3, HazardClass = "Corrosive" };

            _repositoryMock.Setup(r => r.DoesHazardExistByClassAsync(It.IsAny<Hazard>()))
                           .ReturnsAsync(false);
            _repositoryMock.Setup(r => r.DoesHazardExistByIdAsync(It.IsAny<Hazard>()))
                           .ReturnsAsync(true);
            _repositoryMock.Setup(r => r.UpdateHazardAsync(It.IsAny<Hazard>()))
                           .ReturnsAsync(false);

            // Act
            var result = await _service.UpdateHazardAsync(hazard);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            Assert.Equal(ErrorMessage.DbError, result.Message);
            _repositoryMock.Verify(r => r.DoesHazardExistByClassAsync(It.IsAny<Hazard>()), Times.Once);
            _repositoryMock.Verify(r => r.DoesHazardExistByIdAsync(It.IsAny<Hazard>()), Times.Once);
            _repositoryMock.Verify(r => r.UpdateHazardAsync(It.IsAny<Hazard>()), Times.Once);
        }

        [Fact]
        public async Task UpdateHazardAsync_ShouldReturnSuccess_WhenUpdateSucceeds()
        {
            // Arrange
            var hazard = new Hazard { HazardId = 4, HazardClass = "Explosive" };

            _repositoryMock.Setup(r => r.DoesHazardExistByClassAsync(It.IsAny<Hazard>()))
                           .ReturnsAsync(false);
            _repositoryMock.Setup(r => r.DoesHazardExistByIdAsync(It.IsAny<Hazard>()))
                           .ReturnsAsync(true);
            _repositoryMock.Setup(r => r.UpdateHazardAsync(It.IsAny<Hazard>()))
                           .ReturnsAsync(true);

            // Act
            var result = await _service.UpdateHazardAsync(hazard);

            // Assert
            Assert.False(result.IsNotSuccess());
            Assert.Equal(0, result.StatusCode);
            Assert.Null(result.Message);
            _repositoryMock.Verify(r => r.DoesHazardExistByClassAsync(It.IsAny<Hazard>()), Times.Once);
            _repositoryMock.Verify(r => r.DoesHazardExistByIdAsync(It.IsAny<Hazard>()), Times.Once);
            _repositoryMock.Verify(r => r.UpdateHazardAsync(It.IsAny<Hazard>()), Times.Once);
        }
    }
}
