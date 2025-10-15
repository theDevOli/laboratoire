using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;

namespace Laboratoire.Test.Integration.Services;

public class ReportPDFIntegrationTest
{
    private readonly IConfiguration _config;
    private readonly DataContext _dbContext;

    public ReportPDFIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        _dbContext = new DataContext(_config);
    }

    [Fact]
    public async Task GetPhysicalReport_ShouldReturnOK_WhenParametersAreValid()
    {
        var reportRepository = new ReportRepository(_dbContext);
        var protocolRepository = new ProtocolRepository(_dbContext);
        var catalogRepository = new CatalogRepository(_dbContext);
        
        var catalogs = await catalogRepository.GetAllCatalogsAsync();
        var catalogId = catalogs
            .FirstOrDefault(c=>c.LabelName.Equals("Análise Física",StringComparison.InvariantCultureIgnoreCase))
            ?.CatalogId;
        
        var protocols = await protocolRepository.GetAllProtocolsAsync();
        var protocol = protocols.FirstOrDefault(p=>p.CatalogId==catalogId && p.ReportId is not null);
        
        var report = await reportRepository.GetReportByIdAsync(protocol?.ReportId);
        
        
    }
}