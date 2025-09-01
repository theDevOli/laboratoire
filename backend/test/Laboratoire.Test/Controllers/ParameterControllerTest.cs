using Microsoft.AspNetCore.Mvc;
using Moq;

using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.UI.Controllers;

namespace Laboratoire.Tests.Controllers;

public class ParameterControllerTests
{
    private readonly Mock<IParameterGetterService> _parameterGetterServiceMock;
    private readonly Mock<IParameterInputGetterService> _parameterInputGetterServiceMock;
    private readonly Mock<IParameterGetterByIdService> _parameterGetterByIdServiceMock;
    private readonly Mock<IParameterAdderService> _parameterAdderServiceMock;
    private readonly Mock<IParameterUpdatableService> _parameterUpdatableServiceMock;
    private readonly ParameterController _controller;

    public ParameterControllerTests()
    {
        _parameterGetterServiceMock = new Mock<IParameterGetterService>();
        _parameterInputGetterServiceMock = new Mock<IParameterInputGetterService>();
        _parameterGetterByIdServiceMock = new Mock<IParameterGetterByIdService>();
        _parameterAdderServiceMock = new Mock<IParameterAdderService>();
        _parameterUpdatableServiceMock = new Mock<IParameterUpdatableService>();
        _controller = new ParameterController(
            _parameterGetterServiceMock.Object,
            _parameterInputGetterServiceMock.Object,
            _parameterGetterByIdServiceMock.Object,
            _parameterAdderServiceMock.Object,
            _parameterUpdatableServiceMock.Object
        );
    }

    [Fact]
    public async Task GetAllParametersAsync_ReturnsOk_WithParameters()
    {
        // Arrange
        var parameters = new List<Parameter>
        {
            new Parameter { ParameterId = 1,  CatalogId= 1,},
            new Parameter { ParameterId = 2, CatalogId=2 }
        };
        _parameterGetterServiceMock.Setup(service => service.GetAllParametersAsync()).ReturnsAsync(parameters);

        // Act
        var result = await _controller.GetAllParametersAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<IEnumerable<Parameter>>>(okResult.Value);
        Assert.Null(response.Error);
        Assert.Equal(parameters.Count, response.Data?.Count());
    }

    [Fact]
    public async Task GetAllParametersAsync_ShouldReturnServerError_WhenServerErrorOccurs()
    {
        // Arrange
        _parameterGetterServiceMock.Setup(service => service.GetAllParametersAsync()).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.GetAllParametersAsync();

        // Assert
        var resultObject = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, resultObject.StatusCode);
    }

    [Fact]
    public async Task GetParameterByIdAsync_ReturnsParameter_WhenParameterExists()
    {
        // Arrange
        var parameter = new Parameter { ParameterId = 1, CatalogId = 1, };
        _parameterGetterByIdServiceMock.Setup(service => service.GetParameterByIdAsync(It.IsAny<int>())).ReturnsAsync(parameter);

        // Act
        var result = await _controller.GetParameterByIdAsync(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<Parameter>>(okResult.Value);
        Assert.Null(response.Error);
        Assert.Equal(parameter.ParameterId, response.Data?.ParameterId);
    }

    [Fact]
    public async Task GetParameterByIdAsync_ShouldReturnServerError_WhenServerErrorOccurs()
    {
        // Arrange
        _parameterGetterByIdServiceMock.Setup(service => service.GetParameterByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.GetParameterByIdAsync(1);

        // Assert
        var resultObject = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, resultObject.StatusCode);
    }

    [Fact]
    public async Task GetParameterByIdAsync_ReturnsNotFound_WhenParameterDoesNotExist()
    {
        // Arrange
        Parameter? nullParameter = null;
        _parameterGetterByIdServiceMock.Setup(service => service.GetParameterByIdAsync(It.IsAny<int>())).ReturnsAsync(nullParameter);

        // Act
        var result = await _controller.GetParameterByIdAsync(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
        Assert.Equal(404, response.Error?.Code);
        Assert.Null(response.Data);
    }

    [Fact]
    public async Task AddParameterAsync_ReturnsOk_WhenParameterIsAddedSuccessfully()
    {
        // Arrange
        var parameterDto = new ParameterDtoAdd { CatalogId = 1 };
        var addResult = Error.SetSuccess();
        _parameterAdderServiceMock.Setup(service => service.AddParameterAsync(It.IsAny<ParameterDtoAdd>())).ReturnsAsync(addResult);

        // Act
        var result = await _controller.AddParameterAsync(parameterDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
        Assert.Equal(SuccessMessage.Added, response.Data);
        Assert.Null(response.Error);
    }

    [Fact]
    public async Task AddParameterAsync_ReturnsConflict_WhenParameterIsAlreadyRegistered()
    {
        // Arrange
        var parameterDto = new ParameterDtoAdd { CatalogId = 1 };
        var addResult = Error.SetError(ErrorMessage.ConflictPost, 409);
        _parameterAdderServiceMock.Setup(service => service.AddParameterAsync(It.IsAny<ParameterDtoAdd>())).ReturnsAsync(addResult);

        // Act
        var result = await _controller.AddParameterAsync(parameterDto);

        // Assert
        var okResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(okResult.Value);
        Assert.Equal(409, response.Error?.Code);
        Assert.Equal(ErrorMessage.ConflictPost, response.Error?.Message);
        Assert.Null(response.Data);
    }

    [Fact]
    public async Task AddParameterAsync_ShouldReturnServerError_WhenServerErrorOccurs()
    {
        // Arrange
        var parameterDto = new ParameterDtoAdd { CatalogId = 1 };
        _parameterAdderServiceMock.Setup(service => service.AddParameterAsync(It.IsAny<ParameterDtoAdd>())).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.AddParameterAsync(parameterDto);

        // Assert
        var resultObject = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, resultObject.StatusCode);
    }

    [Fact]
    public async Task UpdateParameterAsync_ReturnsOk_WhenParameterIsUpdatedSuccessfully()
    {
        // Arrange
        var parameter = new Parameter { ParameterId = 1, CatalogId = 1 };
        var updateResult = Error.SetSuccess();
        _parameterUpdatableServiceMock.Setup(service => service.UpdateParameterAsync(It.IsAny<Parameter>())).ReturnsAsync(updateResult);

        // Act
        var result = await _controller.UpdateParameterAsync(1, parameter);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
        Assert.Null(response.Error);
        Assert.Equal(SuccessMessage.Added, response.Data);
    }

    [Fact]
    public async Task UpdateParameterAsync_ReturnsBadRequest_WhenIdsDoNotMatch()
    {
        // Arrange
        var parameter = new Parameter { ParameterId = 1, CatalogId = 1 };

        // Act
        var result = await _controller.UpdateParameterAsync(2, parameter);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
        Assert.Equal(ErrorMessage.BadRequestID, response.Error?.Message);
        Assert.Equal(400, response.Error?.Code);
    }

    [Fact]
    public async Task UpdateParameterAsync_ReturnsNotFound_WhenThereIsNoRecordOnTDatabase()
    {
        // Arrange
        var parameter = new Parameter { ParameterId = 1, CatalogId = 1 };
        var updateResult = Error.SetError(ErrorMessage.NotFound, 404);
        _parameterUpdatableServiceMock.Setup(service => service.UpdateParameterAsync(It.IsAny<Parameter>())).ReturnsAsync(updateResult);


        // Act
        var result = await _controller.UpdateParameterAsync(1, parameter);

        // Assert
        var notFoundResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
        Assert.Equal(ErrorMessage.NotFound, response.Error?.Message);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task UpdateParameterAsync_ShouldReturnServerError_WhenServerErrorOccurs()
    {
        // Arrange
        var parameter = new Parameter { ParameterId = 1, CatalogId = 1 };
        int parameterId = 1;
        _parameterUpdatableServiceMock.Setup(service => service.UpdateParameterAsync(It.IsAny<Parameter>())).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.UpdateParameterAsync(parameterId, parameter);

        // Assert
        var resultObject = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, resultObject.StatusCode);
    }
}
