using Laboratoire.Application.DTO;
using Laboratoire.Application.Services.ProtocolServices;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.ProtocolServices
{
    public class ProtocolAdderServiceTest
    {
        private readonly Mock<IProtocolRepository> _protocolRepoMock;
        private readonly Mock<ICropsNormalizationAdderService> _cropsAdderMock;
        private readonly Mock<ICashFlowAdderService> _cashFlowAdderMock;
        private readonly Mock<ILogger<ProtocolAdderService>> _loggerMock;
        private readonly ProtocolAdderService _service;

        public ProtocolAdderServiceTest()
        {
            _protocolRepoMock = new Mock<IProtocolRepository>();
            _cropsAdderMock = new Mock<ICropsNormalizationAdderService>();
            _cashFlowAdderMock = new Mock<ICashFlowAdderService>();
            _loggerMock = new Mock<ILogger<ProtocolAdderService>>();

            _service = new ProtocolAdderService(
                _protocolRepoMock.Object,
                _cropsAdderMock.Object,
                _cashFlowAdderMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task AddProtocolAsync_ShouldReturnConflict_WhenProtocolExists()
        {
            // Arrange
            var dto = new ProtocolDtoAdd();
            _protocolRepoMock.Setup(r => r.DoesProtocolExistByUniqueAsync(It.IsAny<Protocol>()))
                             .ReturnsAsync(true);

            // Act
            var result = await _service.AddProtocolAsync(dto);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(409, result.StatusCode);
            Assert.Equal(ErrorMessage.ConflictPost, result.Message);
            _protocolRepoMock.Verify(r => r.DoesProtocolExistByUniqueAsync(It.IsAny<Protocol>()), Times.Once);
            _protocolRepoMock.Verify(r => r.AddProtocolAsync(It.IsAny<Protocol>()), Times.Never);
            _cashFlowAdderMock.Verify(r => r.AddCashFlowAsync(It.IsAny<CashFlow>(), It.IsAny<Protocol>()), Times.Never);
            _cropsAdderMock.Verify(r => r.AddCropsAsync(It.IsAny<IEnumerable<CropsNormalization>>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task AddProtocolAsync_ShouldReturnDbError_WhenCashFlowFails()
        {
            // Arrange
            var dto = new ProtocolDtoAdd { TotalPaid = 100 };
            _protocolRepoMock.Setup(r => r.DoesProtocolExistByUniqueAsync(It.IsAny<Protocol>()))
                             .ReturnsAsync(false);
            _protocolRepoMock.Setup(r => r.AddProtocolAsync(It.IsAny<Protocol>()))
                             .ReturnsAsync(It.IsAny<string>());
            _cashFlowAdderMock.Setup(c => c.AddCashFlowAsync(It.IsAny<CashFlow>(), It.IsAny<Protocol>()))
                              .ReturnsAsync(Error.SetError("DB Error", 500));

            // Act
            var result = await _service.AddProtocolAsync(dto);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            _protocolRepoMock.Verify(r => r.DoesProtocolExistByUniqueAsync(It.IsAny<Protocol>()), Times.Once);
            _protocolRepoMock.Verify(r => r.AddProtocolAsync(It.IsAny<Protocol>()), Times.Once);
            _cashFlowAdderMock.Verify(r => r.AddCashFlowAsync(It.IsAny<CashFlow>(), It.IsAny<Protocol>()), Times.Once);
            _cropsAdderMock.Verify(r => r.AddCropsAsync(It.IsAny<IEnumerable<CropsNormalization>>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task AddProtocolAsync_ShouldReturnDbError_WhenCropsNormalizationFails()
        {
            // Arrange
            var dto = new ProtocolDtoAdd { TotalPaid = null, Crops = [1, 2] };

            _protocolRepoMock.Setup(r => r.DoesProtocolExistByUniqueAsync(It.IsAny<Protocol>()))
                             .ReturnsAsync(false);
            _protocolRepoMock.Setup(r => r.AddProtocolAsync(It.IsAny<Protocol>()))
                             .ReturnsAsync(It.IsAny<string>());
            _cropsAdderMock.Setup(c => c.AddCropsAsync(It.IsAny<IEnumerable<CropsNormalization>>(), It.IsAny<string>()))
                           .ReturnsAsync(Error.SetError("DB Error", 500));

            // Act
            var result = await _service.AddProtocolAsync(dto);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(500, result.StatusCode);
            _protocolRepoMock.Verify(r => r.DoesProtocolExistByUniqueAsync(It.IsAny<Protocol>()), Times.Once);
            _protocolRepoMock.Verify(r => r.AddProtocolAsync(It.IsAny<Protocol>()), Times.Once);
            _cashFlowAdderMock.Verify(r => r.AddCashFlowAsync(It.IsAny<CashFlow>(), It.IsAny<Protocol>()), Times.Never);
            _cropsAdderMock.Verify(r => r.AddCropsAsync(It.IsAny<IEnumerable<CropsNormalization>>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task AddProtocolAsync_ShouldReturnSuccess_WhenAllSucceeds()
        {
            // Arrange
            var dto = new ProtocolDtoAdd { TotalPaid = 100, Crops = [1, 2] };
            _protocolRepoMock.Setup(r => r.DoesProtocolExistByUniqueAsync(It.IsAny<Protocol>()))
                             .ReturnsAsync(false);
            _protocolRepoMock.Setup(r => r.AddProtocolAsync(It.IsAny<Protocol>()))
                             .ReturnsAsync(It.IsAny<string>());
            _cashFlowAdderMock.Setup(c => c.AddCashFlowAsync(It.IsAny<CashFlow>(), It.IsAny<Protocol>()))
                              .ReturnsAsync(Error.SetSuccess());
            _cropsAdderMock.Setup(c => c.AddCropsAsync(It.IsAny<IEnumerable<CropsNormalization>>(), It.IsAny<string>()))
                           .ReturnsAsync(Error.SetSuccess());

            // Act
            var result = await _service.AddProtocolAsync(dto);

            // Assert
            Assert.False(result.IsNotSuccess());
            Assert.Equal(0, result.StatusCode);
            Assert.Null(result.Message);
            _protocolRepoMock.Verify(r => r.DoesProtocolExistByUniqueAsync(It.IsAny<Protocol>()), Times.Once);
            _protocolRepoMock.Verify(r => r.AddProtocolAsync(It.IsAny<Protocol>()), Times.Once);
            _cashFlowAdderMock.Verify(r => r.AddCashFlowAsync(It.IsAny<CashFlow>(), It.IsAny<Protocol>()), Times.Once);
            _cropsAdderMock.Verify(r => r.AddCropsAsync(It.IsAny<IEnumerable<CropsNormalization>>(), It.IsAny<string>()), Times.Once);
        }
    }
}
