using Laboratoire.Application.Services.ParameterServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ParameterServices;

public class ParameterUpdatableServiceTest
{
    private readonly Mock<IParameterRepository> _repositoryMock;
    private readonly Mock<ILogger<ParameterUpdatableService>> _loggerMock;
    private readonly ParameterUpdatableService _service;

    public ParameterUpdatableServiceTest()
    {
        _repositoryMock = new Mock<IParameterRepository>();
        _loggerMock = new Mock<ILogger<ParameterUpdatableService>>();
        _service = new ParameterUpdatableService(
            _repositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task UpdateParameterAsync_ShouldReturnConflict_WhenParameterNotUnique()
    {
        // Arrange
        var parameter = new Parameter { ParameterId = 1, ParameterName = "pH" };
        _repositoryMock.Setup(r => r.IsParameterUniqueAsync(parameter)).ReturnsAsync(true);

        // Act
        var result = await _service.UpdateParameterAsync(parameter);

        // Assert
        Assert.True(result.IsNotSuccess());
        Assert.Equal(409, result.StatusCode);
        Assert.Equal(ErrorMessage.ConflictPut, result.Message);
        _repositoryMock.Verify(r => r.IsParameterUniqueAsync(parameter), Times.Once);
        _repositoryMock.Verify(r => r.DoesParameterExistByParameterIdAsync(It.IsAny<Parameter>()), Times.Never);
        _repositoryMock.Verify(r => r.UpdateParameterAsync(It.IsAny<Parameter>()), Times.Never);
    }

    [Fact]
    public async Task UpdateParameterAsync_ShouldReturnNotFound_WhenParameterDoesNotExist()
    {
        // Arrange
        var parameter = new Parameter { ParameterId = 2, ParameterName = "Temperature" };
        _repositoryMock.Setup(r => r.IsParameterUniqueAsync(parameter)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.DoesParameterExistByParameterIdAsync(parameter)).ReturnsAsync(false);

        // Act
        var result = await _service.UpdateParameterAsync(parameter);

        // Assert
        Assert.True(result.IsNotSuccess());
        Assert.Equal(404, result.StatusCode);
        Assert.Equal(ErrorMessage.NotFound, result.Message);
        _repositoryMock.Verify(r => r.IsParameterUniqueAsync(parameter), Times.Once);
        _repositoryMock.Verify(r => r.DoesParameterExistByParameterIdAsync(parameter), Times.Once);
        _repositoryMock.Verify(r => r.UpdateParameterAsync(It.IsAny<Parameter>()), Times.Never);
    }

    [Fact]
    public async Task UpdateParameterAsync_ShouldReturnDbError_WhenUpdateFails()
    {
        // Arrange
        var parameter = new Parameter { ParameterId = 3, ParameterName = "Conductivity" };
        _repositoryMock.Setup(r => r.IsParameterUniqueAsync(parameter)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.DoesParameterExistByParameterIdAsync(parameter)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.UpdateParameterAsync(parameter)).ReturnsAsync(false);

        // Act
        var result = await _service.UpdateParameterAsync(parameter);

        // Assert
        Assert.True(result.IsNotSuccess());
        Assert.Equal(500, result.StatusCode);
        Assert.Equal(ErrorMessage.DbError, result.Message);
        _repositoryMock.Verify(r => r.IsParameterUniqueAsync(parameter), Times.Once);
        _repositoryMock.Verify(r => r.DoesParameterExistByParameterIdAsync(parameter), Times.Once);
        _repositoryMock.Verify(r => r.UpdateParameterAsync(parameter), Times.Once);
    }

    [Fact]
    public async Task UpdateParameterAsync_ShouldReturnSuccess_WhenUpdateSucceeds()
    {
        // Arrange
        var parameter = new Parameter { ParameterId = 4, ParameterName = "Turbidity" };
        _repositoryMock.Setup(r => r.IsParameterUniqueAsync(parameter)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.DoesParameterExistByParameterIdAsync(parameter)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.UpdateParameterAsync(parameter)).ReturnsAsync(true);

        // Act
        var result = await _service.UpdateParameterAsync(parameter);

        // Assert
        Assert.False(result.IsNotSuccess());
        Assert.Equal(0, result.StatusCode);
        Assert.Null(result.Message);
        _repositoryMock.Verify(r => r.IsParameterUniqueAsync(parameter), Times.Once);
        _repositoryMock.Verify(r => r.DoesParameterExistByParameterIdAsync(parameter), Times.Once);
        _repositoryMock.Verify(r => r.UpdateParameterAsync(parameter), Times.Once);
    }
}
