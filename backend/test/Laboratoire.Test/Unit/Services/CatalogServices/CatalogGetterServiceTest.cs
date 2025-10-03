using Laboratoire.Application.Services.CatalogServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.CatalogServices
{
    public class CatalogGetterServiceTest
    {
        private readonly Mock<ICatalogRepository> _mockRepo;
        private readonly Mock<ILogger<CatalogGetterService>> _mockLogger;
        private readonly CatalogGetterService _service;

        public CatalogGetterServiceTest()
        {
            _mockRepo = new Mock<ICatalogRepository>();
            _mockLogger = new Mock<ILogger<CatalogGetterService>>();
            _service = new CatalogGetterService(_mockRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllCatalogsAsync_ShouldReturnCatalogs_WhenCatalogsExist()
        {
            // Arrange
            var catalogs = new List<Catalog>
            {
                new Catalog { CatalogId = 1, LabelName = "Cat A" },
                new Catalog { CatalogId = 2, LabelName = "Cat B" }
            };
            _mockRepo.Setup(r => r.GetAllCatalogsAsync()).ReturnsAsync(catalogs);

            // Act
            var result = await _service.GetAllCatalogsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Collection(result,
                c => Assert.Equal("Cat A", c.LabelName),
                c => Assert.Equal("Cat B", c.LabelName));
            _mockRepo.Verify(repo=>repo.GetAllCatalogsAsync(), Times.Once());
        }

        [Fact]
        public async Task GetAllCatalogsAsync_ShouldReturnEmpty_WhenNoCatalogsExist()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetAllCatalogsAsync()).ReturnsAsync(new List<Catalog>());

            // Act
            var result = await _service.GetAllCatalogsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockRepo.Verify(repo=>repo.GetAllCatalogsAsync(), Times.Once());
        }
    }
}
