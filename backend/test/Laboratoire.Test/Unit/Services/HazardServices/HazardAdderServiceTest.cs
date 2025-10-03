using Laboratoire.Application.DTO;
using Laboratoire.Application.Services.HazardServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.HazardServices
{
    public class HazardAdderServiceTest
    {
        private readonly Mock<IHazardRepository> _repositoryMock;
        private readonly Mock<ILogger<HazardAdderService>> _loggerMock;
        private readonly HazardAdderService _service;

        public HazardAdderServiceTest()
        {
            _repositoryMock = new Mock<IHazardRepository>();
            _loggerMock = new Mock<ILogger<HazardAdderService>>();
            _service = new HazardAdderService(
                _repositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task AddHazardAsync_ShouldReturnConflict_WhenHazardAlreadyExists()
        {
            // Arrange
            var hazardDto = new HazardDtoAdd { HazardClass = "Flammable" };

            _repositoryMock.Setup(r => r.DoesHazardExistByClassAsync(It.IsAny<Hazard>()))
                           .ReturnsAsync(true);

            // Act
            var result = await _service.AddHazardAsync(hazardDto);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(409, result.StatusCode);
            Assert.Equal(ErrorMessage.ConflictPost, result.Message);
            _repositoryMock.Verify(r => r.DoesHazardExistByClassAsync(It.IsAny<Hazard>()), Times.Once);
            _repositoryMock.Verify(r => r.AddHazardAsync(It.IsAny<Hazard>()), Times.Never);
        }

        [Fact]
        public async Task AddHazardAsync_ShouldReturnDbError_WhenInsertFails()
        {
            // Arrange
            var hazardDto = new HazardDtoAdd { HazardClass = "Toxic" };

            _repositoryMock.Setup(r => r.DoesHazardExistByClassAsync(It.IsAny<Hazard>()))
                           .ReturnsAsync(false);
            _repositoryMock.Setup(r => r.AddHazardAsync(It.IsAny<Hazard>()))
                           .ReturnsAsync(false);

            // Act
            var result = await _service.AddHazardAsync(hazardDto);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            Assert.Equal(ErrorMessage.DbError, result.Message);
            _repositoryMock.Verify(r => r.DoesHazardExistByClassAsync(It.IsAny<Hazard>()), Times.Once);
            _repositoryMock.Verify(r => r.AddHazardAsync(It.IsAny<Hazard>()), Times.Once);
        }

        [Fact]
        public async Task AddHazardAsync_ShouldReturnSuccess_WhenInsertSucceeds()
        {
            // Arrange
            var hazardDto = new HazardDtoAdd { HazardClass = "Corrosive" };

            _repositoryMock.Setup(r => r.DoesHazardExistByClassAsync(It.IsAny<Hazard>()))
                           .ReturnsAsync(false);
            _repositoryMock.Setup(r => r.AddHazardAsync(It.IsAny<Hazard>()))
                           .ReturnsAsync(true);

            // Act
            var result = await _service.AddHazardAsync(hazardDto);

            // Assert
            Assert.False(result.IsNotSuccess());
            Assert.Equal(0, result.StatusCode);
            Assert.Null(result.Message);
            _repositoryMock.Verify(r => r.DoesHazardExistByClassAsync(It.IsAny<Hazard>()), Times.Once);
            _repositoryMock.Verify(r => r.AddHazardAsync(It.IsAny<Hazard>()), Times.Once);
        }
    }
}
