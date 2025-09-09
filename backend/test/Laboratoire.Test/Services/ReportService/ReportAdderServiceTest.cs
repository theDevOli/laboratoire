using Moq;
using Microsoft.Extensions.Logging;
using Laboratoire.Application.DTO;
using Laboratoire.Application.Services.ReportServices;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Test.Services.ReportServices
{
    public class ReportAdderServiceTest
    {
        private readonly Mock<IReportRepository> _reportRepositoryMock;
        private readonly Mock<IProtocolPatchReportService> _protocolPatchReportServiceMock;
        private readonly Mock<IParameterGetterService> _parameterGetterServiceMock;
        private readonly Mock<ILogger<ReportAdderService>> _loggerMock;
        private readonly ReportAdderService _service;

        public ReportAdderServiceTest()
        {
            _reportRepositoryMock = new Mock<IReportRepository>();
            _protocolPatchReportServiceMock = new Mock<IProtocolPatchReportService>();
            _parameterGetterServiceMock = new Mock<IParameterGetterService>();
            _loggerMock = new Mock<ILogger<ReportAdderService>>();

            _service = new ReportAdderService(
                _reportRepositoryMock.Object,
                _protocolPatchReportServiceMock.Object,
                _parameterGetterServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task AddReportAsync_ShouldReturnSuccess_WhenReportAndProtocolAreAdded()
        {
            // Arrange
            var dto = new ReportDtoAdd { ProtocolId = "0001/2025" };
            var parameters = new List<Parameter>
            {
                new(){ParameterId=1},
                new(){ParameterId=2},
                new(){ParameterId=3},
            };
            _parameterGetterServiceMock.Setup(p => p.GetAllParametersAsync())
                .ReturnsAsync(parameters);

            _reportRepositoryMock.Setup(r => r.AddReportAsync(It.IsAny<Report>()))
                .ReturnsAsync(Guid.NewGuid());

            _protocolPatchReportServiceMock.Setup(p => p.PatchReportIdAsync(It.IsAny<ReportPatch>()))
                .ReturnsAsync(Error.SetSuccess());

            // Act
            var result = await _service.AddReportAsync(dto);

            // Assert
            Assert.False(result.IsNotSuccess());
            _parameterGetterServiceMock.Verify(p => p.GetAllParametersAsync(), Times.Once);
            _reportRepositoryMock.Verify(r => r.AddReportAsync(It.IsAny<Report>()), Times.Once);
            _protocolPatchReportServiceMock.Verify(p => p.PatchReportIdAsync(It.IsAny<ReportPatch>()), Times.Once);
        }

        [Fact]
        public async Task AddReportAsync_ShouldReturnDbError_WhenReportInsertFails()
        {
            // Arrange
            var dto = new ReportDtoAdd { ProtocolId = "0001/2025" };
            var parameters = new List<Parameter>
            {
                new(){ParameterId=1},
                new(){ParameterId=2},
                new(){ParameterId=3},
            };
            _parameterGetterServiceMock.Setup(p => p.GetAllParametersAsync())
                .ReturnsAsync(parameters);
            _reportRepositoryMock.Setup(r => r.AddReportAsync(It.IsAny<Report>()))
                .ReturnsAsync((Guid?)null);

            // Act
            var result = await _service.AddReportAsync(dto);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(ErrorMessage.DbError, result.Message);
            _parameterGetterServiceMock.Verify(p => p.GetAllParametersAsync(), Times.Once);
            _reportRepositoryMock.Verify(r => r.AddReportAsync(It.IsAny<Report>()), Times.Once);
            _protocolPatchReportServiceMock.Verify(p => p.PatchReportIdAsync(It.IsAny<ReportPatch>()), Times.Never);
        }

        [Fact]
        public async Task AddReportAsync_ShouldReturnError_WhenProtocolPatchFails()
        {
            // Arrange
            var dto = new ReportDtoAdd { ProtocolId = "0001/2025" };
            var parameters = new List<Parameter>
            {
                new(){ParameterId=1},
                new(){ParameterId=2},
                new(){ParameterId=3},
            };
            _parameterGetterServiceMock.Setup(p => p.GetAllParametersAsync())
                .ReturnsAsync(parameters);

            _reportRepositoryMock.Setup(r => r.AddReportAsync(It.IsAny<Report>()))
                .ReturnsAsync(Guid.NewGuid());

            _protocolPatchReportServiceMock.Setup(p => p.PatchReportIdAsync(It.IsAny<ReportPatch>()))
                .ReturnsAsync(Error.SetError("Patch failed", 500));

            // Act
            var result = await _service.AddReportAsync(dto);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal("Patch failed", result.Message);
            _parameterGetterServiceMock.Verify(p => p.GetAllParametersAsync(), Times.Once);
            _reportRepositoryMock.Verify(r => r.AddReportAsync(It.IsAny<Report>()), Times.Once);
            _protocolPatchReportServiceMock.Verify(p => p.PatchReportIdAsync(It.IsAny<ReportPatch>()), Times.Once);
        }
    }
}
