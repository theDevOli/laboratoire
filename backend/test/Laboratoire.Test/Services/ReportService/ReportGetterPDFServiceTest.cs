using Laboratoire.Application.DTO;
using Laboratoire.Application.Services.ReportServices;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ReportServices;

public class ReportGetterPDFServiceTest
{
    private readonly Mock<IReportRepository> _reportRepositoryMock;
    private readonly Mock<IParameterRepository> _parameterRepositoryMock;
    private readonly Mock<ICropsNormalizationGetterByReportIdService> _cropsNormalizationGetterMock;
    private readonly Mock<ICropRepository> _cropRepositoryMock;
    private readonly Mock<IFertilizerGetterService> _fertilizerGetterServiceMock;
    private readonly Mock<ILogger<ReportGetterPDFService>> _loggerMock;

    private readonly ReportGetterPDFService _service;

    public ReportGetterPDFServiceTest()
    {
        _reportRepositoryMock = new Mock<IReportRepository>();
        _parameterRepositoryMock = new Mock<IParameterRepository>();
        _cropsNormalizationGetterMock = new Mock<ICropsNormalizationGetterByReportIdService>();
        _cropRepositoryMock = new Mock<ICropRepository>();
        _fertilizerGetterServiceMock = new Mock<IFertilizerGetterService>();
        _loggerMock = new Mock<ILogger<ReportGetterPDFService>>();

        _service = new ReportGetterPDFService(
            _reportRepositoryMock.Object,
            _parameterRepositoryMock.Object,
            _cropsNormalizationGetterMock.Object,
            _cropRepositoryMock.Object,
            _fertilizerGetterServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetReportPDFAsync_ShouldReturnNull_WhenReportOrParametersNotFound()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        _reportRepositoryMock.Setup(r => r.GetReportPDFAsync(reportId))
            .ReturnsAsync((ReportPDF?)null);
        _parameterRepositoryMock.Setup(p => p.GetParametersByReportIdAsync(reportId))
            .ReturnsAsync((IEnumerable<Parameter>?)null);

        // Act
        var result = await _service.GetReportPDFAsync(reportId);

        // Assert
        Assert.Null(result);
        _reportRepositoryMock.Verify(r => r.GetReportPDFAsync(reportId), Times.Once);
        _parameterRepositoryMock.Verify(p => p.GetParametersByReportIdAsync(reportId), Times.Once);
        _cropsNormalizationGetterMock.Verify(c => c.GetCropByReportIdAsync(reportId), Times.Once);
        _cropRepositoryMock.Verify(c => c.GetAllCropsAsync(), Times.Never);
        _fertilizerGetterServiceMock.Verify(f => f.GetAllFertilizersAsync(), Times.Never);
    }

    [Fact]
    public async Task GetReportPDFAsync_ShouldReturnReportPDF_WhenValidDataProvided()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        var reportPdf = new ReportPDF
        {
            ProtocolId = "0001/2025",
            Results =
                [
                    new(){ParameterId=1},
                    new(){ParameterId=2},
                ]
        };
        var crops = new List<Crop>
        {
            new()
            {
                CropId = 1 ,
                CropName ="Test",
                NitrogenCover =20,
                NitrogenFoundation =20,
                Phosphorus=new(){Min=10,Med=15,Max=20},
                Potassium=new(){Min=10,Med=15,Max=20},
                MinV=60,
            }
        };
        var fertilizes = new List<FertilizerDtoGet> { new() { Formulation = "20-20-20" } };
        var parameters = new List<Parameter> { new() { ParameterName = "Fósforo" } };
        var cropsNormalization = new List<CropsNormalization> { new() { CropId = 1 } };

        _reportRepositoryMock.Setup(r => r.GetReportPDFAsync(reportId))
            .ReturnsAsync(reportPdf);
        _parameterRepositoryMock.Setup(p => p.GetParametersByReportIdAsync(reportId))
            .ReturnsAsync(parameters);
        _cropsNormalizationGetterMock.Setup(c => c.GetCropByReportIdAsync(reportId))
            .ReturnsAsync(cropsNormalization);

        _cropRepositoryMock.Setup(c => c.GetAllCropsAsync())
            .ReturnsAsync(crops);

        _fertilizerGetterServiceMock.Setup(f => f.GetAllFertilizersAsync())
            .ReturnsAsync(fertilizes);

        // Act
        var result = await _service.GetReportPDFAsync(reportId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Suggestions);
        Assert.Equal(result.ProtocolId, reportPdf.ProtocolId);
        _reportRepositoryMock.Verify(r => r.GetReportPDFAsync(reportId), Times.Once);
        _parameterRepositoryMock.Verify(p => p.GetParametersByReportIdAsync(reportId), Times.Once);
        _cropsNormalizationGetterMock.Verify(c => c.GetCropByReportIdAsync(reportId), Times.Once);
        _cropRepositoryMock.Verify(c => c.GetAllCropsAsync(), Times.Once);
        _fertilizerGetterServiceMock.Verify(f => f.GetAllFertilizersAsync(), Times.Once);
    }

    [Fact]
    public async Task GetReportPDFAsync_ShouldCalculateSuggestions_WhenNoCropsNormalizationExists()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        var reportPdf = new ReportPDF
        {
            ProtocolId = "0001/2025",
            Results =
                [
                    new(){ParameterId=1},
                    new(){ParameterId=2},
                ]
        };
        var crops = new List<Crop> { new() { CropId = 1 } };
        var fertilizes = new List<FertilizerDtoGet> { new() { Formulation = "20-20-20" } };
        var parameters = new List<Parameter> { new() { ParameterName = "Fósforo" } };
        var cropsNormalization = Enumerable.Empty<CropsNormalization>;

        _reportRepositoryMock.Setup(r => r.GetReportPDFAsync(reportId))
            .ReturnsAsync(reportPdf);
        _parameterRepositoryMock.Setup(p => p.GetParametersByReportIdAsync(reportId))
            .ReturnsAsync(parameters);
        _cropsNormalizationGetterMock.Setup(c => c.GetCropByReportIdAsync(reportId))
            .ReturnsAsync(cropsNormalization);

        _cropRepositoryMock.Setup(c => c.GetAllCropsAsync())
            .ReturnsAsync(crops);

        _fertilizerGetterServiceMock.Setup(f => f.GetAllFertilizersAsync())
            .ReturnsAsync(fertilizes);

        // Act
        var result = await _service.GetReportPDFAsync(reportId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Suggestions);
        _reportRepositoryMock.Verify(r => r.GetReportPDFAsync(reportId), Times.Once);
        _parameterRepositoryMock.Verify(p => p.GetParametersByReportIdAsync(reportId), Times.Once);
        _cropsNormalizationGetterMock.Verify(c => c.GetCropByReportIdAsync(reportId), Times.Once);
        _cropRepositoryMock.Verify(c => c.GetAllCropsAsync(), Times.Never);
        _fertilizerGetterServiceMock.Verify(f => f.GetAllFertilizersAsync(), Times.Never);
    }
}
