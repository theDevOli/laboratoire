using Laboratoire.Application.Services.ParameterServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ParameterServices;

public class ParameterGetterByIdServiceTest
{
    private readonly Mock<IParameterRepository> _repositoryMock;
    private readonly Mock<ILogger<ParameterGetterByIdService>> _loggerMock;
    private readonly ParameterGetterByIdService _service;

    public ParameterGetterByIdServiceTest()
    {
        _repositoryMock = new Mock<IParameterRepository>();
        _loggerMock = new Mock<ILogger<ParameterGetterByIdService>>();
        _service = new ParameterGetterByIdService(
            _repositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetParameterByIdAsync_ShouldReturnNull_WhenParameterIdIsNull()
    {
        // Act
        var result = await _service.GetParameterByIdAsync(null);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(r => r.GetParameterByParameterIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetParameterByIdAsync_ShouldReturnParameter_WhenParameterExists()
    {
        // Arrange
        int parameterId = 1;
        var parameter = new Parameter { ParameterId = parameterId, ParameterName = "pH" };
        _repositoryMock.Setup(r => r.GetParameterByParameterIdAsync(It.IsAny<int>()))
                       .ReturnsAsync(parameter);

        // Act
        var result = await _service.GetParameterByIdAsync(parameterId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(parameterId, result!.ParameterId);
        Assert.Equal("pH", result.ParameterName);
        _repositoryMock.Verify(r => r.GetParameterByParameterIdAsync(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task GetParameterByIdAsync_ShouldReturnNull_WhenParameterDoesNotExist()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetParameterByParameterIdAsync(It.IsAny<int>()))
                       .ReturnsAsync((Parameter?)null);

        // Act
        var result = await _service.GetParameterByIdAsync(999);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(r => r.GetParameterByParameterIdAsync(It.IsAny<int>()), Times.Once);
    }
}

