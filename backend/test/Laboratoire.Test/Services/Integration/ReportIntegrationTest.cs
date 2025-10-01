using Laboratoire.Application.Services.ReportServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace Laboratoire.Test.Services.Integration;

public class ReportIntegrationTest
{
    private readonly IConfiguration _config;
    private readonly DataContext _dbContext;

    public ReportIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        _dbContext = new DataContext(_config);
    }
    [Fact]
    public async Task PatchReport_ShouldSucceed()
    {
        // Arrange
        var repository = new ReportRepository(_dbContext);
        var service = new ReportPatchService(repository, NullLogger<ReportPatchService>.Instance);

        var report = new Report
        {
            Results =
            [
                new() { ParameterId = 1, ValueA = 10.0, ValueB = 20.0, Equation = "test" }
            ]
        };

        var reportId = await repository.AddReportAsync(report);

        var toUpdateReport = new Report
        {
            ReportId = reportId,
            Results =
            [
                new() { ParameterId = 1, ValueA = 10.0, ValueB = 20.0, Equation = "Updated" }
            ]
        };

        // Act
        var result = await service.PatchReportAsync(toUpdateReport);
        var updatedReport = await repository.GetReportByIdAsync(reportId);
        // Assert
        Assert.False(result.IsNotSuccess());
        Assert.NotNull(updatedReport);
        Assert.NotNull(updatedReport.Results);
        Assert.Collection(toUpdateReport.Results,
            item => Assert.Equal(item.Equation, updatedReport.Results[0].Equation)
        );
    }
}
