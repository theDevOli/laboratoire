using Laboratoire.Application.Services.ReportServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ReportServices;

public class ReportGetterServiceTest
{
    private readonly Mock<IReportRepository> _reportRepositoryMock;
    private readonly Mock<ILogger<ReportGetterService>> _loggerMock;
    private readonly ReportGetterService _service;

    public ReportGetterServiceTest()
    {
        _reportRepositoryMock = new Mock<IReportRepository>();
        _loggerMock = new Mock<ILogger<ReportGetterService>>();
        _service = new ReportGetterService(
            _reportRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetAllReportsAsync_ShouldReturnReports_WhenRepositoryReturnsData()
    {
        // Arrange
        var expectedReports = new List<Report>
        {
            new() { ReportId = Guid.NewGuid()},
            new() { ReportId = Guid.NewGuid() }
        };

        _reportRepositoryMock.Setup(r => r.GetAllReportsAsync())
            .ReturnsAsync(expectedReports);

        // Act
        var result = await _service.GetAllReportsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Collection
        (
            result,
            item => Assert.Equal(item.ReportId, expectedReports[0].ReportId),
            item => Assert.Equal(item.ReportId, expectedReports[1].ReportId)
        );
        _reportRepositoryMock.Verify(r => r.GetAllReportsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllReportsAsync_ShouldReturnEmpty_WhenRepositoryReturnsEmpty()
    {
        // Arrange
        _reportRepositoryMock.Setup(r => r.GetAllReportsAsync())
            .ReturnsAsync(Enumerable.Empty<Report>());

        // Act
        var result = await _service.GetAllReportsAsync();

        // Assert
        Assert.Empty(result);
        _reportRepositoryMock.Verify(r => r.GetAllReportsAsync(), Times.Once);
    }
}
