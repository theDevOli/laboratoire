using Laboratoire.Application.Services.ReportServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ReportServices;

public class ReportGetterByIdServiceTest
{
    private readonly Mock<IReportRepository> _reportRepositoryMock;
    private readonly Mock<ILogger<ReportGetterByIdService>> _loggerMock;
    private readonly ReportGetterByIdService _service;

    public ReportGetterByIdServiceTest()
    {
        _reportRepositoryMock = new Mock<IReportRepository>();
        _loggerMock = new Mock<ILogger<ReportGetterByIdService>>();
        _service = new ReportGetterByIdService(_reportRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetReportByIdAsync_ShouldReturnReport_WhenReportExists()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        var expectedReport = new Report { ReportId = reportId };
        _reportRepositoryMock
            .Setup(r => r.GetReportByIdAsync(reportId))
            .ReturnsAsync(expectedReport);

        // Act
        var result = await _service.GetReportByIdAsync(reportId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(reportId, result!.ReportId);
        _reportRepositoryMock.Verify(r => r.GetReportByIdAsync(reportId), Times.Once);
    }

    [Fact]
    public async Task GetReportByIdAsync_ShouldReturnNull_WhenReportDoesNotExist()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        _reportRepositoryMock
            .Setup(r => r.GetReportByIdAsync(reportId))
            .ReturnsAsync((Report?)null);

        // Act
        var result = await _service.GetReportByIdAsync(reportId);

        // Assert
        Assert.Null(result);
        _reportRepositoryMock.Verify(r => r.GetReportByIdAsync(reportId), Times.Once);
    }

    [Fact]
    public async Task GetReportByIdAsync_ShouldReturnNull_WhenReportIdIsNull()
    {
        // Arrange
        var reportId = (Guid?)null;
        _reportRepositoryMock
            .Setup(r => r.GetReportByIdAsync(reportId))
            .ReturnsAsync((Report?)null);

        // Act
        var result = await _service.GetReportByIdAsync(reportId);

        // Assert
        Assert.Null(result);
        _reportRepositoryMock.Verify(r => r.GetReportByIdAsync(reportId), Times.Never);
    }


}
