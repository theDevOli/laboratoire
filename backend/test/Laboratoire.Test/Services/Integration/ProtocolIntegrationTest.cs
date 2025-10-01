using System;
using Laboratoire.Application.DTO;
using Laboratoire.Application.Services.CashFlowServices;
using Laboratoire.Application.Services.CropServices;
using Laboratoire.Application.Services.ParameterServices;
using Laboratoire.Application.Services.ProtocolServices;
using Laboratoire.Application.Services.ReportServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using NuGet.Frameworks;

namespace Laboratoire.Test.Services.Integration;

public class ProtocolIntegrationTest
{
    private readonly IConfiguration _config;
    private readonly DataContext _dbContext;

    public ProtocolIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        _dbContext = new DataContext(_config);
    }

    [Fact]
    public async Task UpdateProtocol_ShouldSucceed()
    {
        //  Arrange
        var protocolRepository = new ProtocolRepository(_dbContext);
        var reportRepository = new ReportRepository(_dbContext);
        var parameterRepository = new ParameterRepository(_dbContext);
        var cashFlowRepository = new CashFlowRepository(_dbContext);
        var cropsNormalizationRepository = new CropsNormalizationRepository(_dbContext);

        var propertyRepository = new PropertyRepository(_dbContext);

        var protocolPatchReportService = new ProtocolPatchReportService(protocolRepository, NullLogger<ProtocolPatchReportService>.Instance);
        var cropsNormalizationDeleterService = new CropsNormalizationDeleterService
                                                (cropsNormalizationRepository, NullLogger<CropsNormalizationDeleterService>.Instance);

        var protocolPatchCatalogService = new ProtocolPatchCatalogService
                                            (protocolRepository, reportRepository, NullLogger<ProtocolPatchCatalogService>.Instance);
        var parameterGetterService = new ParameterGetterService(parameterRepository, NullLogger<ParameterGetterService>.Instance);
        var reportAdderService = new ReportAdderService
                (
                    reportRepository,
                    protocolPatchReportService,
                    parameterGetterService,
                    NullLogger<ReportAdderService>.Instance
                );
        var reportPatchService = new ReportPatchService(reportRepository, NullLogger<ReportPatchService>.Instance);
        var cashFlowUpdatableService = new CashFlowUpdatableService(cashFlowRepository, NullLogger<CashFlowUpdatableService>.Instance);
        var cropsNormalizationAdderService = new CropsNormalizationAdderService
                            (cropsNormalizationRepository, cropsNormalizationDeleterService, NullLogger<CropsNormalizationAdderService>.Instance);
        var cashFlowAdderService = new CashFlowAdderService(cashFlowRepository, protocolRepository, NullLogger<CashFlowAdderService>.Instance);
        var service = new ProtocolUpdatableService
            (
                protocolRepository,
                protocolPatchCatalogService,
                reportAdderService,
                reportPatchService,
                cashFlowUpdatableService,
                cropsNormalizationAdderService,
                cashFlowAdderService,
                NullLogger<ProtocolUpdatableService>.Instance
            );

        var properties = await propertyRepository.GetAllPropertiesAsync();
        var property = properties.First();

        Protocol protocol = new()
        {
            CashFlowId = default,
            ReportId = default,
            ClientId = property.ClientId,
            PropertyId = property.PropertyId,
            PartnerId = default,
            CatalogId = 1,
            EntryDate = DateTime.Now,
            ReportDate = DateTime.Now.AddDays(3),
            IsCollectedByClient = true
        };

        var protocolId = await protocolRepository.AddProtocolAsync(protocol);

        ProtocolDtoUpdate toUpdateProtocol = new()
        {
            ProtocolId = protocolId,
            CashFlowId = default,
            ReportId = default,
            ClientId = property.ClientId,
            PropertyId = property.PropertyId,
            PartnerId = default,
            CatalogId = 1,
            EntryDate = DateTime.Now,
            ReportDate = DateTime.Now.AddDays(3),
            IsCollectedByClient = false,
        };

        //  Act
        var result = await service.UpdateProtocolAsync(toUpdateProtocol);
        var updatedProtocol = await protocolRepository.GetProtocolByProtocolIdAsync(protocolId);

        // Assert
        Assert.False(result.IsNotSuccess());
        Assert.NotNull(updatedProtocol);
        Assert.NotNull(updatedProtocol.ReportId);
        Assert.Equal(toUpdateProtocol.IsCollectedByClient, updatedProtocol.IsCollectedByClient);

        // Clean up
        await protocolRepository.DeleteProtocolAsync(protocolId);
    }
}
