using Laboratoire.Application.DTO;
using Laboratoire.Application.Services.ParameterServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ParameterServices;

public class ParameterAdderServiceTest
{
    private readonly Mock<IParameterRepository> _repositoryMock;
    private readonly Mock<ILogger<ParameterAdderService>> _loggerMock;
    private readonly ParameterAdderService _service;

    public ParameterAdderServiceTest()
    {
        _repositoryMock = new Mock<IParameterRepository>();
        _loggerMock = new Mock<ILogger<ParameterAdderService>>();
        _service = new ParameterAdderService(
            _repositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task AddParameterAsync_ShouldReturnConflict_WhenParameterAlreadyExists()
    {
        // Arrange
        var parameterDto = new ParameterDtoAdd { ParameterName = "pH" };

        _repositoryMock.Setup(r => r.IsParameterUniqueAsync(It.IsAny<Domain.Entity.Parameter>()))
                       .ReturnsAsync(true);

        // Act
        var result = await _service.AddParameterAsync(parameterDto);

        // Assert
        Assert.True(result.IsNotSuccess());
        Assert.Equal(409, result.StatusCode);
        Assert.Equal(ErrorMessage.ConflictPost, result.Message);
        _repositoryMock.Verify(r => r.IsParameterUniqueAsync(It.IsAny<Domain.Entity.Parameter>()), Times.Once);
        _repositoryMock.Verify(r => r.AddParameterAsync(It.IsAny<Domain.Entity.Parameter>()), Times.Never);
    }

    [Fact]
    public async Task AddParameterAsync_ShouldReturnDbError_WhenInsertFails()
    {
        // Arrange
        var parameterDto = new ParameterDtoAdd { ParameterName = "Conductivity" };

        _repositoryMock.Setup(r => r.IsParameterUniqueAsync(It.IsAny<Domain.Entity.Parameter>()))
                       .ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AddParameterAsync(It.IsAny<Domain.Entity.Parameter>()))
                       .ReturnsAsync(false);

        // Act
        var result = await _service.AddParameterAsync(parameterDto);

        // Assert
        Assert.True(result.IsNotSuccess());
        Assert.Equal(500, result.StatusCode);
        Assert.Equal(ErrorMessage.DbError, result.Message);
        _repositoryMock.Verify(r => r.IsParameterUniqueAsync(It.IsAny<Domain.Entity.Parameter>()), Times.Once);
        _repositoryMock.Verify(r => r.AddParameterAsync(It.IsAny<Domain.Entity.Parameter>()), Times.Once);
    }

    [Fact]
    public async Task AddParameterAsync_ShouldReturnSuccess_WhenInsertSucceeds()
    {
        // Arrange
        var parameterDto = new ParameterDtoAdd { ParameterName = "Temperature" };

        _repositoryMock.Setup(r => r.IsParameterUniqueAsync(It.IsAny<Domain.Entity.Parameter>()))
                       .ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AddParameterAsync(It.IsAny<Domain.Entity.Parameter>()))
                       .ReturnsAsync(true);

        // Act
        var result = await _service.AddParameterAsync(parameterDto);

        // Assert
        Assert.False(result.IsNotSuccess());
        Assert.Equal(0, result.StatusCode);
        Assert.Null(result.Message);
        _repositoryMock.Verify(r => r.IsParameterUniqueAsync(It.IsAny<Domain.Entity.Parameter>()), Times.Once);
        _repositoryMock.Verify(r => r.AddParameterAsync(It.IsAny<Domain.Entity.Parameter>()), Times.Once);
    }
}

