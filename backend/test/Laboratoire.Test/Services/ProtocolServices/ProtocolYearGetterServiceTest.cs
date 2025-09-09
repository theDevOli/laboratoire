using Laboratoire.Application.DTO;
using Laboratoire.Application.Services.ProtocolServices;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ProtocolServices;

public class ProtocolYearGetterServiceTests
{
    private readonly Mock<IProtocolRepository> _repoMock;
    private readonly Mock<ILogger<ProtocolYearGetterService>> _loggerMock;
    private readonly ProtocolYearGetterService _service;

    public ProtocolYearGetterServiceTests()
    {
        _repoMock = new Mock<IProtocolRepository>();
        _loggerMock = new Mock<ILogger<ProtocolYearGetterService>>();
        _service = new ProtocolYearGetterService(_repoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetProtocolYearsAsync_ShouldReturnExpectedYears_WhenYearsExists()
    {
        var expectedYears = new List<ProtocolDtoYears>
        {
            new ProtocolDtoYears { Year = 2024 },
            new ProtocolDtoYears { Year = 2025 }
        };

        _repoMock.Setup(r => r.GetProtocolYearsAsync<ProtocolDtoYears>())
                 .ReturnsAsync(expectedYears);

        var result = await _service.GetProtocolYearsAsync();

        Assert.Collection
        (
            result,
            item => Assert.Equal(item.Year, expectedYears[0].Year),
            item => Assert.Equal(item.Year, expectedYears[1].Year)
        );
        _repoMock.Verify(r => r.GetProtocolYearsAsync<ProtocolDtoYears>(), Times.Once);
    }

    [Fact]
    public async Task GetProtocolYearsAsync_ShouldReturnEmptyCollection_WhenNoYearsExists()
    {

        _repoMock.Setup(r => r.GetProtocolYearsAsync<ProtocolDtoYears>())
                 .ReturnsAsync(Enumerable.Empty<ProtocolDtoYears>());

        var result = await _service.GetProtocolYearsAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
        _repoMock.Verify(r => r.GetProtocolYearsAsync<ProtocolDtoYears>(), Times.Once);
    }
}
