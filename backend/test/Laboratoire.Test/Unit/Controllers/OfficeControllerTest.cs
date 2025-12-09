using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.UI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Laboratoire.Test.Unit.Controllers;

public class OfficeControllerTest
{
    private readonly Mock<IOfficeAdderService> _mockAdderService;
    private readonly Mock<IOfficeGetterByIdService> _mockGetterByIdService;
    private readonly Mock<IOfficeGetterService> _mockGetterService;
    private readonly Mock<IOfficeUpdatableService> _mockUpdatableService;
    private readonly OfficeController _controller;

    public OfficeControllerTest()
    {
        _mockAdderService = new();
        _mockGetterByIdService = new();
        _mockGetterService = new();
        _mockUpdatableService = new();

        _controller = new OfficeController
        (
            _mockAdderService.Object,
            _mockGetterByIdService.Object,
            _mockGetterService.Object,
            _mockUpdatableService.Object
        );
    }

    [Fact]
    public async Task GetAllOfficesAsync_ShouldReturnAllOffices_WhenOfficesExist()
    {
        // Arrange
        var offices = new List<Office>()
        {
            new(){ OfficeId= Guid.NewGuid(),OfficeEmail="test1@email.com",OfficeName="Test1",City="Test1"},
            new(){ OfficeId= Guid.NewGuid(),OfficeEmail="test2@email.com",OfficeName="Test2",City="Test2"},
            new(){ OfficeId= Guid.NewGuid(),OfficeEmail="test3@email.com",OfficeName="Test3",City="Test3"},
            new(){ OfficeId= Guid.NewGuid(),OfficeEmail="test4@email.com",OfficeName="Test4",City="Test4"},
        };

        _mockGetterService.Setup(repo => repo.GetAllOfficesAsync()).ReturnsAsync(offices);

        // Act
        var result = await _controller.GetAllOfficesAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<IEnumerable<Office>>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Null(response.Error);
        Assert.Collection
        (
            response.Data,
             item => Assert.Equal(offices[0].OfficeId, item.OfficeId),
             item => Assert.Equal(offices[1].OfficeId, item.OfficeId),
             item => Assert.Equal(offices[2].OfficeId, item.OfficeId),
             item => Assert.Equal(offices[3].OfficeId, item.OfficeId)
        );
        _mockGetterService.Verify(repo => repo.GetAllOfficesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllOfficesAsync_ShouldReturnEmptyList_WhenNoOfficeExists()
    {
        // Arrange
        _mockGetterService.Setup(repo => repo.GetAllOfficesAsync()).ReturnsAsync(Enumerable.Empty<Office>());

        // Act
        var result = await _controller.GetAllOfficesAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<IEnumerable<Office>>>(okResult.Value);
        Assert.Empty(response.Data!);
        Assert.Null(response.Error);
        _mockGetterService.Verify(repo => repo.GetAllOfficesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetOfficeByOfficeIdAsync_ShouldReturnOffice_WhenOfficeExists()
    {
        // Arrange
        var officeId = Guid.NewGuid();
        Office office = new() { OfficeId = officeId, OfficeEmail = "test1@email.com", OfficeName = "Test1", City = "Test1" };

        _mockGetterByIdService.Setup(repo => repo.GetByOfficeIdAsync(It.IsAny<Guid>())).ReturnsAsync(office);

        // Act
        var result = await _controller.GetOfficeByOfficeIdAsync(officeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<Office>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Null(response.Error);
        Assert.Equal(response.Data.OfficeId, officeId);
        _mockGetterByIdService.Verify(repo => repo.GetByOfficeIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task GetOfficeByOfficeIdAsync_ShouldReturnNull_WhenNoOfficeExists()
    {
        // Arrange
        var officeId = Guid.NewGuid();

        _mockGetterByIdService.Setup(repo => repo.GetByOfficeIdAsync(It.IsAny<Guid>())).ReturnsAsync((Office?)null);

        // Act
        var result = await _controller.GetOfficeByOfficeIdAsync(officeId);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(objectResult.Value);
        Assert.Null(response.Data);
        Assert.Equal(404, response.Error?.Code);
        _mockGetterByIdService.Verify(repo => repo.GetByOfficeIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task AddOfficeAsync_ShouldReturnConflict_WhenOfficeExistsOnDatabase()
    {
        // Arrange
        var dto = new OfficeDtoUpsert() { OfficeEmail = "test1@email.com", OfficeName = "Test1", City = "Test1" };
        var error = Error.SetError(ErrorMessage.ConflictPost, 409);

        _mockAdderService.Setup(repo => repo.AddOfficeAsync(It.IsAny<Office>())).ReturnsAsync(error);

        // Act
        var result = await _controller.AddOfficeAsync(dto);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(objectResult.Value);
        Assert.Null(response.Data);
        Assert.Equal(409, response.Error?.Code);
        _mockAdderService.Verify(repo => repo.AddOfficeAsync(It.IsAny<Office>()), Times.Once);
    }

    [Fact]
    public async Task AddOfficeAsync_ShouldReturnCreated_WhenOperationSucceeds()
    {
        // Arrange
        var dto = new OfficeDtoUpsert() { OfficeEmail = "test1@email.com", OfficeName = "Test1", City = "Test1" };
        var expectedError = Error.SetSuccess();

        _mockAdderService.Setup(repo => repo.AddOfficeAsync(It.IsAny<Office>())).ReturnsAsync(expectedError);
        // Act
        var result = await _controller.AddOfficeAsync(dto);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<ApiResponse<string>>(objectResult.Value);
        Assert.NotNull(response.Data);
        Assert.Equal(SuccessMessage.Added, response.Data);
        Assert.Null(response.Error);
        _mockAdderService.Verify(repo => repo.AddOfficeAsync(It.IsAny<Office>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOfficeAsync_ShouldReturnNotFound_WhenNoOfficeExistsOnDataBase()
    {
        // Arrange
        Guid officeId = Guid.NewGuid();
        var officeDto = new OfficeDtoUpsert() {  OfficeEmail = "test1@email.com", OfficeName = "Test1", City = "Test1" };
        var expectedError = Error.SetError(ErrorMessage.NotFound, 404);

        _mockUpdatableService.Setup(repo => repo.UpdateOfficeAsync(It.IsAny<Office>())).ReturnsAsync(expectedError);
        // Act
        var result = await _controller.UpdateOfficeAsync(officeDto, officeId);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(objectResult.Value);
        Assert.Null(response.Data);
        Assert.NotNull(response.Error);
        Assert.Equal(ErrorMessage.NotFound, response.Error.Message);
        _mockUpdatableService.Verify(repo => repo.UpdateOfficeAsync(It.IsAny<Office>()), Times.Once);
    }
    
    [Fact]
    public async Task UpdateOfficeAsync_ShouldReturnOk_WhenOperationsSucceed()
    {
        // Arrange
        Guid officeId = Guid.NewGuid();
        var officeDto = new OfficeDtoUpsert() { OfficeEmail = "test1@email.com", OfficeName = "Test1", City = "Test1" };
        var expectedError = Error.SetSuccess();

        _mockUpdatableService.Setup(repo => repo.UpdateOfficeAsync(It.IsAny<Office>())).ReturnsAsync(expectedError);
        // Act
        var result = await _controller.UpdateOfficeAsync(officeDto,officeId);

        // Assert
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<string>>(objectResult.Value);
        Assert.NotNull(response.Data);
        Assert.Equal(SuccessMessage.Updated, response.Data);
        Assert.Null(response.Error);
        _mockUpdatableService.Verify(repo => repo.UpdateOfficeAsync(It.IsAny<Office>()), Times.Once);
    }
}
