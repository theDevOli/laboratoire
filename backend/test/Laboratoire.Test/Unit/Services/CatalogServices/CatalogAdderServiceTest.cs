using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;
using Laboratoire.Application.Services.CatalogServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.CatalogServices
{
    public class CatalogAdderServiceTest
    {
        private readonly Mock<ICatalogRepository> _catalogRepositoryMock;
        private readonly Mock<ILogger<CatalogAdderService>> _loggerMock;
        private readonly CatalogAdderService _service;

        public CatalogAdderServiceTest()
        {
            _catalogRepositoryMock = new Mock<ICatalogRepository>();
            _loggerMock = new Mock<ILogger<CatalogAdderService>>();

            _service = new CatalogAdderService(
                _catalogRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task AddCatalogAsync_ShouldReturnConflict_WhenCatalogAlreadyExists()
        {
            // Arrange
            var catalogDto = new CatalogDtoAdd { LabelName = "TestCatalog" };
            var catalog = catalogDto.ToCatalog();

            _catalogRepositoryMock
                .Setup(r => r.DoesCatalogExistByUniqueAsync(It.IsAny<Catalog>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.AddCatalogAsync(catalogDto);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(ErrorMessage.ConflictPost, result.Message);
            Assert.Equal(409, result.StatusCode);

            _catalogRepositoryMock.Verify(r => r.DoesCatalogExistByUniqueAsync(It.Is<Catalog>(c => c.LabelName == "TestCatalog")), Times.Once);
            _catalogRepositoryMock.Verify(r => r.AddCatalogAsync(It.IsAny<Catalog>()), Times.Never);
        }

        [Fact]
        public async Task AddCatalogAsync_ShouldReturnConflict_WhenInsertFails()
        {
            // Arrange
            var catalogDto = new CatalogDtoAdd { LabelName = "InsertFailCatalog" };
            var catalog = catalogDto.ToCatalog();

            _catalogRepositoryMock
                .Setup(r => r.DoesCatalogExistByUniqueAsync(It.IsAny<Catalog>()))
                .ReturnsAsync(false);

            _catalogRepositoryMock
                .Setup(r => r.AddCatalogAsync(It.IsAny<Catalog>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.AddCatalogAsync(catalogDto);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(ErrorMessage.ConflictPost, result.Message);
            Assert.Equal(409, result.StatusCode);

            _catalogRepositoryMock.Verify(r => r.DoesCatalogExistByUniqueAsync(It.Is<Catalog>(c => c.LabelName == "InsertFailCatalog")), Times.Once);
            _catalogRepositoryMock.Verify(r => r.AddCatalogAsync(It.Is<Catalog>(c => c.LabelName == "InsertFailCatalog")), Times.Once);
        }

        [Fact]
        public async Task AddCatalogAsync_ShouldReturnSuccess_WhenInsertSucceeds()
        {
            // Arrange
            var catalogDto = new CatalogDtoAdd { LabelName = "ValidCatalog" };
            var catalog = catalogDto.ToCatalog();

            _catalogRepositoryMock
                .Setup(r => r.DoesCatalogExistByUniqueAsync(It.IsAny<Catalog>()))
                .ReturnsAsync(false);

            _catalogRepositoryMock
                .Setup(r => r.AddCatalogAsync(It.IsAny<Catalog>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.AddCatalogAsync(catalogDto);

            // Assert
            Assert.False(result.IsNotSuccess());
            Assert.Null(result.Message);
            Assert.Equal(0, result.StatusCode);

            _catalogRepositoryMock.Verify(r => r.DoesCatalogExistByUniqueAsync(It.Is<Catalog>(c => c.LabelName == "ValidCatalog")), Times.Once);
            _catalogRepositoryMock.Verify(r => r.AddCatalogAsync(It.Is<Catalog>(c => c.LabelName == "ValidCatalog")), Times.Once);
        }
    }
}
