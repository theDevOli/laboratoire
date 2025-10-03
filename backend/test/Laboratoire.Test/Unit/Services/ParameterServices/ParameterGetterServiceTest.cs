using Laboratoire.Application.Services.ParameterServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ParameterServices;

public class ParameterGetterServiceTest
{
    private readonly Mock<IParameterRepository> _repositoryMock;
    private readonly Mock<ILogger<ParameterGetterService>> _loggerMock;
    private readonly ParameterGetterService _service;

    public ParameterGetterServiceTest()
    {
        _repositoryMock = new Mock<IParameterRepository>();
        _loggerMock = new Mock<ILogger<ParameterGetterService>>();
        _service = new ParameterGetterService(
            _repositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetAllParametersAsync_ShouldReturnEmptyList_WhenNoParametersExist()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllParametersAsync())
                       .ReturnsAsync(Enumerable.Empty<Parameter>());

        // Act
        var result = await _service.GetAllParametersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _repositoryMock.Verify(r => r.GetAllParametersAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllParametersAsync_ShouldReturnListOfParameters_WhenParametersExist()
    {
        // Arrange
        var parameters = new List<Parameter>
            {
                new Parameter { ParameterId = 1, ParameterName = "pH" },
                new Parameter { ParameterId = 2, ParameterName = "Temperature" }
            };

        _repositoryMock.Setup(r => r.GetAllParametersAsync())
                       .ReturnsAsync(parameters);

        // Act
        var result = await _service.GetAllParametersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Collection
        (
            parameters,
            p => Assert.Equal(p.ParameterId, parameters[0].ParameterId),
            p => Assert.Equal(p.ParameterId, parameters[1].ParameterId)
        );
        _repositoryMock.Verify(r => r.GetAllParametersAsync(), Times.Once);
    }
}

