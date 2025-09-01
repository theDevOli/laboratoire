
using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.UI.Controllers;
using Laboratoire.Application.Utils;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Laboratoire.Tests.Controllers;

public class HazardControllerTests
{
    private readonly Mock<IHazardGetterService> _hazardGetterServiceMock;
    private readonly Mock<IHazardGetterByIdService> _hazardGetterByIdServiceMock;
    private readonly Mock<IHazardAdderService> _hazardAdderServiceMock;
    private readonly Mock<IHazardUpdatableService> _hazardUpdatableServiceMock;
    private readonly HazardController _controller;

    public HazardControllerTests()
    {
        _hazardGetterServiceMock = new Mock<IHazardGetterService>();
        _hazardGetterByIdServiceMock = new Mock<IHazardGetterByIdService>();
        _hazardAdderServiceMock = new Mock<IHazardAdderService>();
        _hazardUpdatableServiceMock = new Mock<IHazardUpdatableService>();
        _controller = new HazardController(
            _hazardGetterServiceMock.Object,
            _hazardGetterByIdServiceMock.Object,
            _hazardAdderServiceMock.Object,
            _hazardUpdatableServiceMock.Object
        );
    }

    [Fact]
    public async Task GetAllHazard_ReturnsOk_WithHazards()
    {
        // Arrange
        var hazards = new List<Hazard> {
             new Hazard { HazardId = 1, HazardClass = "Class 1", HazardName = "Name 1" },
             new Hazard { HazardId = 2, HazardClass = "Class 2", HazardName = "Name 2" },
             new Hazard { HazardId = 3, HazardClass = "Class 3", HazardName = "Name 3" },
             new Hazard { HazardId = 4, HazardClass = "Class 4", HazardName = "Name 4" },
              };
        _hazardGetterServiceMock.Setup(service => service.GetAllHazardsAsync()).ReturnsAsync(hazards);

        // Act
        var result = await _controller.GetAllHazard();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<IEnumerable<Hazard>>>(okResult.Value);
        Assert.Null(response.Error);
        Assert.Equal(hazards.Count(), response.Data?.Count());
    }

    [Fact]
    public async Task GetAllHazard_ShouldReturnServerError_WhenSeverErrorOccurs()
    {
        // Arrange
        _hazardGetterServiceMock.Setup(repo => repo.GetAllHazardsAsync()).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.GetAllHazard();

        // Assert
        var okResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

        Assert.Equal(500, response.Error?.Code);
        Assert.Null(response.Data);
    }

    [Fact]
    public async Task GetHazardByIdAsync_ReturnsNotFound_WhenHazardDoesNotExist()
    {
        // Arrange
        Hazard? nullHazard = null;
        _hazardGetterByIdServiceMock.Setup(service => service.GetHazardByIdAsync(It.IsAny<int>())).ReturnsAsync(nullHazard);

        // Act
        var result = await _controller.GetHazardByIdAsync(1);

        // Assert
        var notFoundObject = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(notFoundObject.Value);
        Assert.Equal(404, response.Error?.Code);
        Assert.Equal(ErrorMessage.NotFound, response.Error?.Message);
        Assert.Null(response.Data);
    }

    [Fact]
    public async Task GetHazardByIdAsync_ReturnsHazard_WhenHazardIsValid()
    {
        // Arrange
        var hazard = new Hazard { HazardId = 1, HazardClass = "Class 1", HazardName = "Name 1" };
        _hazardGetterByIdServiceMock.Setup(service => service.GetHazardByIdAsync(It.IsAny<int>())).ReturnsAsync(hazard);

        // Act
        var result = await _controller.GetHazardByIdAsync(1);

        // Assert
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<Hazard>>(objectResult.Value);
        Assert.Null(response.Error);
        Assert.Equal(hazard.HazardName, response.Data?.HazardName);
    }

    [Fact]
    public async Task GetHazardByIdAsync_ShouldReturnServerError_WhenSeverErrorOccurs()
    {
        // Arrange
        _hazardGetterByIdServiceMock.Setup(repo => repo.GetHazardByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.GetHazardByIdAsync(1);

        // Assert
        var okResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

        Assert.Equal(500, response.Error?.Code);
        Assert.Null(response.Data);
    }

    [Fact]
    public async Task AddHazardAsync_ReturnsCreated_WhenHazardIsAddedSuccessfully()
    {
        // Arrange
        var hazardDto = new HazardDtoAdd { HazardClass = "Class 1", HazardName = "Name 1" };
        var addResult = Error.SetSuccess();
        _hazardAdderServiceMock.Setup(service => service.AddHazardAsync(hazardDto)).ReturnsAsync(addResult);

        // Act
        var result = await _controller.AddHazardAsync(hazardDto);

        // Assert
        var createdResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<ApiResponse<string>>(createdResult.Value);
        Assert.Equal(201, createdResult.StatusCode);
        Assert.Equal(SuccessMessage.Added, response.Data);
        Assert.Null(response.Error);
    }

    [Fact]
    public async Task AddHazardAsync_ReturnsConflict_WhenHazardHasSameClass()
    {
        // Arrange
        var hazardDto = new HazardDtoAdd { HazardClass = "Class 1", HazardName = "Name 1" };
        var addResult = Error.SetError(ErrorMessage.ConflictPost, 409);
        _hazardAdderServiceMock.Setup(service => service.AddHazardAsync(It.IsAny<HazardDtoAdd>())).ReturnsAsync(addResult);

        // Act
        var result = await _controller.AddHazardAsync(hazardDto);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(objectResult.Value);
        Assert.Equal(409, objectResult.StatusCode);
        Assert.Equal(409, response.Error?.Code);
        Assert.Equal(ErrorMessage.ConflictPost, response.Error?.Message);
        Assert.Null(response.Data);
    }


    [Fact]
    public async Task AddHazardAsync_ShouldReturnServerError_WhenSeverErrorOccurs()
    {
        // Arrange
        var hazardDto = new HazardDtoAdd { HazardClass = "Class 1", HazardName = "Name 1" };
        _hazardAdderServiceMock.Setup(repo => repo.AddHazardAsync(It.IsAny<HazardDtoAdd>())).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.AddHazardAsync(hazardDto);

        // Assert
        var okResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

        Assert.Equal(500, response.Error?.Code);
        Assert.Null(response.Data);
    }

    [Fact]
    public async Task UpdateHazardAsync_ReturnsOk_WhenHazardIsUpdatedSuccessfully()
    {
        // Arrange
        var hazard = new Hazard { HazardId = 1, HazardClass = "Class 1", HazardName = "Name 1" };
        var updateResult = Error.SetSuccess();
        _hazardUpdatableServiceMock.Setup(service => service.UpdateHazardAsync(hazard)).ReturnsAsync(updateResult);

        // Act
        var result = await _controller.UpdateHazardAsync(1, hazard);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
        Assert.Null(response.Error);
        Assert.Equal(SuccessMessage.Updated, response.Data);
    }

    [Fact]
    public async Task UpdateHazardAsync_ReturnsBadRequest_WhenIdsDoNotMatch()
    {
        // Arrange
        var hazard = new Hazard { HazardId = 1, HazardClass = "Class 1", HazardName = "Name 1" };

        // Act
        var result = await _controller.UpdateHazardAsync(2, hazard);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
        Assert.Null(response.Data);
        Assert.Equal(ErrorMessage.BadRequestID, response.Error?.Message);
        Assert.Equal(400, response.Error?.Code);
    }

    [Fact]
    public async Task UpdateHazardAsync_ReturnsNotFound_WhenUpdateFails()
    {
        // Arrange
        var hazard = new Hazard { HazardId = 1, HazardClass = "Class 1", HazardName = "Name 1" };
        var updateError = Error.SetError(ErrorMessage.NotFound, 404);
        _hazardUpdatableServiceMock.Setup(service => service.UpdateHazardAsync(hazard)).ReturnsAsync(updateError);

        // Act
        var result = await _controller.UpdateHazardAsync(1, hazard);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(objectResult.Value);
        Assert.Null(response.Data);
        // Assert.Equal(ErrorMessage.Conflict, response.Error?.Message);
        Assert.Equal(404, response.Error?.Code);
    }

    [Fact]
    public async Task UpdateHazardAsync_ShouldReturnServerError_WhenSeverErrorOccurs()
    {
        // Arrange
        var hazard = new Hazard { HazardId = 1, HazardClass = "Class 1", HazardName = "Name 1" };
        var hazardId = 1;
        _hazardUpdatableServiceMock.Setup(repo => repo.UpdateHazardAsync(It.IsAny<Hazard>())).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.UpdateHazardAsync(hazardId, hazard);

        // Assert
        var okResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

        Assert.Equal(500, response.Error?.Code);
        Assert.Null(response.Data);
    }
}

