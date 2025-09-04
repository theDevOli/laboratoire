using Laboratoire.Application.Services.CropServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.CropServices
{
    public class CropsNormalizationGetterByReportIdServiceTest
    {
        private readonly Mock<ICropsNormalizationRepository> _repositoryMock;
        private readonly Mock<ILogger<CropsNormalizationGetterByReportIdService>> _loggerMock;
        private readonly CropsNormalizationGetterByReportIdService _service;

        public CropsNormalizationGetterByReportIdServiceTest()
        {
            _repositoryMock = new Mock<ICropsNormalizationRepository>();
            _loggerMock = new Mock<ILogger<CropsNormalizationGetterByReportIdService>>();
            _service = new CropsNormalizationGetterByReportIdService(
                _repositoryMock.Object, _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetCropByReportIdAsync_ShouldReturnNull_WhenReportIdIsNull()
        {
            // Act
            var result = await _service.GetCropByReportIdAsync(null);

            // Assert
            Assert.Null(result);
            _repositoryMock.Verify(r => r.GetCropByReportIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetCropByReportIdAsync_ShouldReturnCrops_WhenReportIdIsValid()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var crops = new List<CropsNormalization>
            {
                new CropsNormalization { ProtocolId = "P1" },
                new CropsNormalization { ProtocolId = "P2" }
            };

            _repositoryMock
                .Setup(r => r.GetCropByReportIdAsync(reportId))
                .ReturnsAsync(crops);

            // Act
            var result = await _service.GetCropByReportIdAsync(reportId);

            // Assert
            Assert.NotNull(result);
            Assert.Collection
            (
                result,
                item => Assert.Equal(item.ProtocolId, crops[0].ProtocolId),
                item=>Assert.Equal(item.ProtocolId,crops[1].ProtocolId)
            );
            _repositoryMock.Verify(r => r.GetCropByReportIdAsync(It.IsAny<Guid>()), Times.Once);
        }
    }
}
