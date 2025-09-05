using Laboratoire.Application.Services.PartnerServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.PartnerServices
{
    public class PartnerUpdatableServiceTest
    {
        private readonly Mock<IPartnerRepository> _repositoryMock;
        private readonly Mock<ILogger<PartnerUpdatableService>> _loggerMock;
        private readonly PartnerUpdatableService _service;

        public PartnerUpdatableServiceTest()
        {
            _repositoryMock = new Mock<IPartnerRepository>();
            _loggerMock = new Mock<ILogger<PartnerUpdatableService>>();
            _service = new PartnerUpdatableService(
                _repositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task UpdatePartnerAsync_ShouldReturnNotFound_WhenPartnerDoesNotExist()
        {
            // Arrange
            var partner = new Partner { PartnerId = Guid.NewGuid(), PartnerName = "Test", PartnerEmail = "test@test.com" };
            _repositoryMock.Setup(r => r.DoesPartnerExistByIdAsync(partner))
                           .ReturnsAsync(false);

            // Act
            var result = await _service.UpdatePartnerAsync(partner);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(404, result.StatusCode);
            Assert.Equal(ErrorMessage.NotFound, result.Message);
            _repositoryMock.Verify(r => r.DoesPartnerExistByIdAsync(partner), Times.Once);
            _repositoryMock.Verify(r => r.UpdatePartnerAsync(It.IsAny<Partner>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePartnerAsync_ShouldReturnDbError_WhenUpdateFails()
        {
            // Arrange
            var partner = new Partner { PartnerId = Guid.NewGuid(), PartnerName = "Test", PartnerEmail = "test@test.com" };
            _repositoryMock.Setup(r => r.DoesPartnerExistByIdAsync(partner))
                           .ReturnsAsync(true);
            _repositoryMock.Setup(r => r.UpdatePartnerAsync(partner))
                           .ReturnsAsync(false);

            // Act
            var result = await _service.UpdatePartnerAsync(partner);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            Assert.Equal(ErrorMessage.DbError, result.Message);
            _repositoryMock.Verify(r => r.DoesPartnerExistByIdAsync(partner), Times.Once);
            _repositoryMock.Verify(r => r.UpdatePartnerAsync(partner), Times.Once);
        }

        [Fact]
        public async Task UpdatePartnerAsync_ShouldReturnSuccess_WhenUpdateSucceeds()
        {
            // Arrange
            var partner = new Partner { PartnerId = Guid.NewGuid(), PartnerName = "Test", PartnerEmail = "test@test.com" };
            _repositoryMock.Setup(r => r.DoesPartnerExistByIdAsync(partner))
                           .ReturnsAsync(true);
            _repositoryMock.Setup(r => r.UpdatePartnerAsync(partner))
                           .ReturnsAsync(true);

            // Act
            var result = await _service.UpdatePartnerAsync(partner);

            // Assert
            Assert.False(result.IsNotSuccess());
            Assert.Equal(0, result.StatusCode);
            Assert.Null(result.Message);
            _repositoryMock.Verify(r => r.DoesPartnerExistByIdAsync(partner), Times.Once);
            _repositoryMock.Verify(r => r.UpdatePartnerAsync(partner), Times.Once);
        }
    }
}
