using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Laboratoire.Test.Integration.Repositories;

public class ReportRepositoryIntegrationTest
{
    private readonly ReportRepository _repository;
    private readonly NpgsqlConnection _connection;

    public ReportRepositoryIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        var dbContext = new Infrastructure.DbContext.DataContext(config);
        var connectionString = config.GetConnectionString("DefaultConnectionDev");

        _repository = new ReportRepository(dbContext);
        _connection = new NpgsqlConnection(connectionString);
    }

    [Fact]
    public async Task GetAllReportsAsync_ShouldSucceed()
    {
        // Act
        var reports = await _repository.GetAllReportsAsync();

        // Assert
        Assert.NotEmpty(reports);
        Assert.IsAssignableFrom<IEnumerable<Report>>(reports);
    }

    [Fact]
    public async Task GetReportByIdAsync_ShouldSucceed()
    {
        // Arrange
        var firstReport = (await _repository.GetAllReportsAsync()).FirstOrDefault();

        // Act
        var result = await _repository.GetReportByIdAsync(firstReport?.ReportId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Report>(result);
    }

    [Fact]
    public async Task GetReportPDFAsync_ShouldSucceed()
    {
        // Arrange
        var firstReport = (await _repository.GetAllReportsAsync()).FirstOrDefault();

        // Act
        var result = await _repository.GetReportPDFAsync(firstReport?.ReportId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ReportPDF>(result);
    }

    [Fact]
    public async Task DoesReportExistsAsync_ShouldSucceed()
    {
        // Arrange
        var firstReport = (await _repository.GetAllReportsAsync()).FirstOrDefault();

        // Act
        var result = await _repository.DoesReportExistsAsync(firstReport!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AddReportAsync_ShouldSucceed()
    {
        // Arrange
        var firstReport = (await _repository.GetAllReportsAsync()).FirstOrDefault();
        var newReport = new Report()
        {
            ReportId = default,
            Results = firstReport?.Results
        };

        // Act
        var reportId = await _repository.AddReportAsync(firstReport!);

        // Assert
        Assert.NotNull(reportId);

        // Tear down
        await _connection.ExecuteAsync("DELETE FROM document.report WHERE report_id = @reportId", new { reportId });
    }

    [Fact]
    public async Task DeleteReportAsync_ShouldSucceed()
    {
        // Arrange
        var firstReport = (await _repository.GetAllReportsAsync()).FirstOrDefault();
        var newReport = new Report()
        {
            ReportId = default,
            Results = firstReport?.Results
        };

        var reportId = await _repository.AddReportAsync(newReport!);

        // Act
        var result = await _repository.DeleteReportAsync(reportId);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task PatchReportAsync_ShouldSucceed()
    {
        // Arrange
        var reports = (await _repository.GetAllReportsAsync()).ToArray();
        var newReport = new Report()
        {
            ReportId = default,
            Results = reports[0]?.Results
        };

        var reportId = await _repository.AddReportAsync(newReport!);

        var toUpdate = new Report()
        {
            ReportId = reportId,
            Results = reports[1]?.Results
        };

        // Act
        var result = await _repository.PatchReportAsync(toUpdate);
        var updated = await _repository.GetReportByIdAsync(reportId);

        // Assert
        Assert.True(result);
        Assert.Collection
        (
            toUpdate.Results!, 
            item => Assert.Equal(item.ParameterId,updated?.Results?[0].ParameterId),
            item => Assert.Equal(item.ParameterId,updated?.Results?[1].ParameterId),
            item => Assert.Equal(item.ParameterId,updated?.Results?[2].ParameterId),
            item => Assert.Equal(item.ParameterId,updated?.Results?[3].ParameterId),
            item => Assert.Equal(item.ParameterId,updated?.Results?[4].ParameterId),
            item => Assert.Equal(item.ParameterId,updated?.Results?[5].ParameterId),
            item => Assert.Equal(item.ParameterId,updated?.Results?[6].ParameterId),
            item => Assert.Equal(item.ParameterId,updated?.Results?[7].ParameterId),
            item => Assert.Equal(item.ParameterId,updated?.Results?[8].ParameterId),
            item => Assert.Equal(item.ParameterId,updated?.Results?[9].ParameterId),
            item => Assert.Equal(item.ParameterId,updated?.Results?[10].ParameterId)
        );

        // Tear down
        await _connection.ExecuteAsync("DELETE FROM document.report WHERE report_id = @reportId", new { reportId });
    }

    [Fact]
    public async Task ResetReportAsync_ShouldSucceed()
    {
        // Arrange
        var reports = (await _repository.GetAllReportsAsync()).ToArray();
        var newReport = new Report()
        {
            ReportId = default,
            Results = reports[0]?.Results
        };

        var reportId = await _repository.AddReportAsync(newReport!);

        // Act
        var result = await _repository.ResetReportAsync(reportId);
        var resetReport = await _repository.GetReportByIdAsync(reportId);

        // Assert
        Assert.True(result);
        Assert.Null(resetReport?.Results);

        // Tear down
        await _connection.ExecuteAsync("DELETE FROM document.report WHERE report_id = @reportId", new { reportId });
    }
}
