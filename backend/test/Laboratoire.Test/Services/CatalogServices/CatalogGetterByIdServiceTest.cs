using Laboratoire.Application.Services.CatalogServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.CatalogServices
{
    public class CatalogGetterByIdServiceTest
    {
        private readonly Mock<ICatalogRepository> _mockRepo;
        private readonly Mock<ILogger<CatalogGetterByIdService>> _mockLogger;
        private readonly CatalogGetterByIdService _service;

        public CatalogGetterByIdServiceTest()
        {
            _mockRepo = new Mock<ICatalogRepository>();
            _mockLogger = new Mock<ILogger<CatalogGetterByIdService>>();
            _service = new CatalogGetterByIdService(_mockRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetCatalogByIdAsync_ShouldReturnNull_WhenCatalogIdIsNull()
        {
            // Act
            var result = await _service.GetCatalogByIdAsync(null);

            // Assert
            Assert.Null(result);
            _mockRepo.Verify(r => r.GetCatalogByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetCatalogByIdAsync_ShouldReturnCatalog_WhenCatalogExists()
        {
            // Arrange
            var catalog = new Catalog { CatalogId = 1, LabelName = "Test Catalog" };
            _mockRepo.Setup(r => r.GetCatalogByIdAsync(1))
                     .ReturnsAsync(catalog);

            // Act
            var result = await _service.GetCatalogByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.CatalogId);
            Assert.Equal("Test Catalog", result.LabelName);
            _mockRepo.Verify(r => r.GetCatalogByIdAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GetCatalogByIdAsync_ShouldReturnNull_WhenCatalogNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetCatalogByIdAsync(99))
                     .ReturnsAsync((Catalog?)null);

            // Act
            var result = await _service.GetCatalogByIdAsync(99);

            // Assert
            Assert.Null(result);
            _mockRepo.Verify(r => r.GetCatalogByIdAsync(It.IsAny<int>()), Times.Once);
        }
    }
}
