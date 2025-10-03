using Laboratoire.Application.Services.ReportServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ReportServices;

public class ReportPatchServiceTest
{
    private readonly Mock<IReportRepository> _reportRepositoryMock;
    private readonly Mock<ILogger<ReportPatchService>> _loggerMock;
    private readonly ReportPatchService _service;

    public ReportPatchServiceTest()
    {
        _reportRepositoryMock = new Mock<IReportRepository>();
        _loggerMock = new Mock<ILogger<ReportPatchService>>();
        _service = new ReportPatchService(_reportRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task PatchReportAsync_ShouldReturnError_WhenReportDoesNotExist()
    {
        // Arrange
        var report = new Report { ReportId = Guid.NewGuid() };
        _reportRepositoryMock.Setup(r => r.DoesReportExistsAsync(report)).ReturnsAsync(false);

        // Act
        var result = await _service.PatchReportAsync(report);

        // Assert
        Assert.True(result.IsNotSuccess());
        Assert.Equal(404, result.StatusCode);
        _reportRepositoryMock.Verify(r => r.DoesReportExistsAsync(It.IsAny<Report>()), Times.Once);
        _reportRepositoryMock.Verify(r => r.PatchReportAsync(It.IsAny<Report>()), Times.Never);
    }

    [Fact]
    public async Task PatchReportAsync_ShouldReturnError_WhenPatchFails()
    {
        // Arrange
        var report = new Report { ReportId = Guid.NewGuid() };
        _reportRepositoryMock.Setup(r => r.DoesReportExistsAsync(report)).ReturnsAsync(true);
        _reportRepositoryMock.Setup(r => r.PatchReportAsync(report)).ReturnsAsync(false);

        // Act
        var result = await _service.PatchReportAsync(report);

        // Assert
        Assert.True(result.IsNotSuccess());
        Assert.Equal(500, result.StatusCode);
        _reportRepositoryMock.Verify(r => r.DoesReportExistsAsync(It.IsAny<Report>()), Times.Once);
        _reportRepositoryMock.Verify(r => r.PatchReportAsync(It.IsAny<Report>()), Times.Once);
    }

    [Fact]
    public async Task PatchReportAsync_ShouldReturnSuccess_WhenPatchSucceeds()
    {
        // Arrange
        var report = new Report { ReportId = Guid.NewGuid() };
        _reportRepositoryMock.Setup(r => r.DoesReportExistsAsync(report)).ReturnsAsync(true);
        _reportRepositoryMock.Setup(r => r.PatchReportAsync(report)).ReturnsAsync(true);

        // Act
        var result = await _service.PatchReportAsync(report);

        // Assert
        Assert.False(result.IsNotSuccess());
        _reportRepositoryMock.Verify(r => r.DoesReportExistsAsync(It.IsAny<Report>()), Times.Once);
        _reportRepositoryMock.Verify(r => r.PatchReportAsync(It.IsAny<Report>()), Times.Once);
    }
}
