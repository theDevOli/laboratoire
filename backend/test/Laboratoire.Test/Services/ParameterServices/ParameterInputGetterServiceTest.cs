using Laboratoire.Application.DTO;
using Laboratoire.Application.Services.ParameterServices;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ParameterServices;

public class ParameterInputGetterServiceTest
{
    private readonly Mock<IParameterRepository> _repositoryMock;
    private readonly Mock<ILogger<ParameterInputGetterService>> _loggerMock;
    private readonly ParameterInputGetterService _service;

    public ParameterInputGetterServiceTest()
    {
        _repositoryMock = new Mock<IParameterRepository>();
        _loggerMock = new Mock<ILogger<ParameterInputGetterService>>();
        _service = new ParameterInputGetterService(
            _repositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetParameterInputByIdAsync_ShouldReturnNull_WhenCatalogIdIsNull()
    {
        // Act
        var result = await _service.GetParameterInputByIdAsync(null);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(r => r.GetParametersInputByCatalogIdAsync<ParameterDtoDisplay>(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetParameterInputByIdAsync_ShouldReturnParameters_WhenCatalogIdIsValid()
    {
        // Arrange
        int catalogId = 1;
        var parameters = new List<ParameterDtoDisplay>
            {
                new ParameterDtoDisplay { ParameterId = 1, ParameterName = "pH" },
                new ParameterDtoDisplay { ParameterId = 2, ParameterName = "Temperature" }
            };

        _repositoryMock
            .Setup(r => r.GetParametersInputByCatalogIdAsync<ParameterDtoDisplay>(catalogId))
            .ReturnsAsync(parameters);

        // Act
        var result = await _service.GetParameterInputByIdAsync(catalogId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Collection
        (
            result,
            item => Assert.Equal(item.ParameterId, parameters[0].ParameterId),
            item => Assert.Equal(item.ParameterId, parameters[1].ParameterId)
        );
        _repositoryMock.Verify(r => r.GetParametersInputByCatalogIdAsync<ParameterDtoDisplay>(catalogId), Times.Once);
    }
}
