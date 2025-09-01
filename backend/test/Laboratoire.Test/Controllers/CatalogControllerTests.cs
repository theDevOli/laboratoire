using Microsoft.AspNetCore.Mvc;
using Moq;

using Laboratoire.Domain.Entity;
using Laboratoire.Application.Utils;
using Laboratoire.UI.Controllers;
using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;

namespace Laboratoire.Tests.Controllers
{
    public class CatalogControllerTests
    {
        private readonly Mock<ICatalogGetterService> _mockCatalogGetter;
        private readonly Mock<ICatalogGetterByIdService> _mockCatalogGetterById;
        private readonly Mock<ICatalogAdderService> _mockCatalogAdder;
        private readonly Mock<ICatalogUpdatableService> _mockCatalogUpdatable;
        private readonly CatalogController _controller;

        public CatalogControllerTests()
        {
            _mockCatalogGetter = new Mock<ICatalogGetterService>();
            _mockCatalogGetterById = new Mock<ICatalogGetterByIdService>();
            _mockCatalogAdder = new Mock<ICatalogAdderService>();
            _mockCatalogUpdatable = new Mock<ICatalogUpdatableService>();
            _controller = new CatalogController
            (_mockCatalogGetter.Object, _mockCatalogGetterById.Object, _mockCatalogAdder.Object, _mockCatalogUpdatable.Object);
        }

        [Fact]
        public async Task GetAllCatalogsAsync_ShouldReturnOkResult_WithListOfCatalogs()
        {
            // Arrange
            var catalogs = new List<Catalog>
            {
                new Catalog { CatalogId = 1, ReportType = "Catalog1",SampleType= "Catalog1",Price=10.25m},
                new Catalog { CatalogId = 2, ReportType = "Catalog2",SampleType= "Catalog2",Price=20.25m},
            };
            _mockCatalogGetter.Setup(repo => repo.GetAllCatalogsAsync()).ReturnsAsync(catalogs);

            // Act
            var result = await _controller.GetAllCatalogsAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<Catalog>>>(okResult.Value);
            Assert.Equal(catalogs.Count, response.Data?.Count());
        }

        [Fact]
        public async Task GetAllCatalogsAsync_ShouldReturnServerError_WhenSeverErrorOccurs()
        {
            // Arrange
            _mockCatalogGetter.Setup(repo => repo.GetAllCatalogsAsync()).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetAllCatalogsAsync();

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

            Assert.Equal(500, response.Error?.Code);
            Assert.Null(response.Data);
        }

        [Fact]
        public async Task GetCatalogByIdAsync_ShouldReturnOk_WhenCatalogExist()
        {
            // Arrange
            int catalogId = 1;
            Catalog catalog = new Catalog { CatalogId = 1, ReportType = "Catalog1", SampleType = "Catalog1", Price = 10.25m };
            _mockCatalogGetterById.Setup(repo => repo.GetCatalogByIdAsync(It.IsAny<int>())).ReturnsAsync(catalog);

            // Act
            var result = await _controller.GetCatalogByIdAsync(catalogId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<Catalog>>(okResult.Value);
            Assert.Null(response.Error);
            Assert.Equal(catalog.CatalogId, response.Data?.CatalogId);
        }

        [Fact]
        public async Task GetCatalogByIdAsync_ShouldReturnServerError_WhenSeverErrorOccurs()
        {
            // Arrange
            _mockCatalogGetterById.Setup(repo => repo.GetCatalogByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetCatalogByIdAsync(1);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

            Assert.Equal(500, response.Error?.Code);
            Assert.Null(response.Data);
        }

        [Fact]
        public async Task GetCatalogByIdAsync_ShouldReturnNotFound_WhenCatalogDoesNotExist()
        {
            // Arrange
            int catalogId = 1;
            Catalog? catalog = null;
            _mockCatalogGetterById.Setup(repo => repo.GetCatalogByIdAsync(catalogId)).ReturnsAsync(catalog);

            // Act
            var result = await _controller.GetCatalogByIdAsync(catalogId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Null(response.Data);
            Assert.Equal(ErrorMessage.NotFound, response.Error?.Message);
            Assert.Equal(404, response.Error?.Code);
        }

        [Fact]
        public async Task AddCatalogAsync_ShouldReturnOkResult_WhenCatalogIsAddedSuccessfully()
        {
            // Arrange
            var expectedError = Error.SetSuccess();
            var catalogDto = new CatalogDtoAdd { ReportType = "Catalog1", SampleType = "Catalog1", Price = 10.25m };
            _mockCatalogAdder.Setup(repo => repo.AddCatalogAsync(It.IsAny<CatalogDtoAdd>())).ReturnsAsync(expectedError);

            // Act
            var result = await _controller.AddCatalogAsync(catalogDto);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal(SuccessMessage.Added, response.Data);
            Assert.Null(response.Error);
        }

        [Fact]
        public async Task AddCatalogAsync_ShouldReturnConflict_WhenCatalogAlreadyExists()
        {
            // Arrange
            var expectedError = Error.SetError(ErrorMessage.ConflictPost, 409);
            var catalogDto = new CatalogDtoAdd { ReportType = "Catalog1", SampleType = "Catalog1", Price = 10.25m };
            _mockCatalogAdder.Setup(repo => repo.AddCatalogAsync(It.IsAny<CatalogDtoAdd>())).ReturnsAsync(expectedError);

            // Act
            var result = await _controller.AddCatalogAsync(catalogDto);

            // Assert
            var conflictResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(conflictResult.Value);
            Assert.Null(response.Data);
            Assert.Equal(ErrorMessage.ConflictPost, response.Error?.Message);
            Assert.Equal(409, response.Error?.Code);
        }

        [Fact]
        public async Task AddCatalogAsync_ShouldReturnServerError_WhenSeverErrorOccurs()
        {
            // Arrange
            var catalogDto = new CatalogDtoAdd { ReportType = "Catalog1", SampleType = "Catalog1", Price = 10.25m };
            _mockCatalogAdder.Setup(repo => repo.AddCatalogAsync(It.IsAny<CatalogDtoAdd>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.AddCatalogAsync(catalogDto);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

            Assert.Equal(500, response.Error?.Code);
            Assert.Null(response.Data);
        }

        [Fact]
        public async Task UpdateCatalogAsync_ShouldReturnBadRequest_WhenIdsDoNotMatch()
        {
            // Arrange
            int catalogId = 2;
            var catalog = new Catalog { CatalogId = 1, ReportType = "Catalog1", SampleType = "Catalog1", Price = 10.25m };

            // Act
            var result = await _controller.UpdateCatalogsAsync(catalogId, catalog);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Null(response.Data);
            Assert.Equal(400, response.Error?.Code);
            Assert.Equal(ErrorMessage.BadRequestID, response.Error?.Message);
        }

        [Fact]
        public async Task UpdateCatalogAsync_ShouldReturnOkResult_WhenUpdateIsSuccessful()
        {
            // Arrange
            int catalogId = 1;
            var catalog = new Catalog { CatalogId = 1, ReportType = "Catalog1", SampleType = "Catalog1", Price = 10.25m };
            var expectedError = Error.SetSuccess();
            _mockCatalogUpdatable.Setup(repo => repo.UpdateCatalogAsync(catalog)).ReturnsAsync(expectedError);

            // Act
            var result = await _controller.UpdateCatalogsAsync(catalogId, catalog);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Null(response.Error);
            Assert.Equal(SuccessMessage.Updated, response.Data);
        }

        [Fact]
        public async Task UpdateCatalogAsync_ShouldReturnNotFound_WhenCatalogDoesNotExist()
        {
            // Arrange
            int catalogId = 1;
            var catalog = new Catalog { CatalogId = 1, ReportType = "Catalog1", SampleType = "Catalog1", Price = 10.25m };
            var expectedError = Error.SetError(ErrorMessage.NotFound, 404);
            _mockCatalogUpdatable.Setup(repo => repo.UpdateCatalogAsync(catalog)).ReturnsAsync(expectedError);

            // Act
            var result = await _controller.UpdateCatalogsAsync(catalogId, catalog);

            // Assert
            var notFoundResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Null(response.Data);
            Assert.Equal(ErrorMessage.NotFound, response.Error?.Message);
            Assert.Equal(404, response.Error?.Code);
        }

        [Fact]
        public async Task UpdateCatalogAsync_ShouldReturnServerError_WhenSeverErrorOccurs()
        {
            // Arrange
            int catalogId = 1;
            var catalog = new Catalog { CatalogId = 1, ReportType = "Catalog1", SampleType = "Catalog1", Price = 10.25m };
            _mockCatalogUpdatable.Setup(repo => repo.UpdateCatalogAsync(It.IsAny<Catalog>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.UpdateCatalogsAsync(catalogId, catalog);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

            Assert.Equal(500, response.Error?.Code);
            Assert.Null(response.Data);
        }

    }
}
