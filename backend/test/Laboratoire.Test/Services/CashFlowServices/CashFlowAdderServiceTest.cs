
using Laboratoire.Application.Services.CashFlowServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.CashFlowServices
{
    public class CashFlowAdderServiceTest
    {
        private readonly Mock<ICashFlowRepository> _cashFlowRepositoryMock;
        private readonly Mock<IProtocolRepository> _protocolRepositoryMock;
        private readonly Mock<ILogger<CashFlowAdderService>> _loggerMock;
        private readonly CashFlowAdderService _service;

        public CashFlowAdderServiceTest()
        {
            _cashFlowRepositoryMock = new Mock<ICashFlowRepository>();
            _protocolRepositoryMock = new Mock<IProtocolRepository>();
            _loggerMock = new Mock<ILogger<CashFlowAdderService>>();

            _service = new CashFlowAdderService(
                _cashFlowRepositoryMock.Object,
                _protocolRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task AddCashFlowAsync_ShouldReturnSuccess_WhenCashFlowAddedAndProtocolUpdated()
        {
            // Arrange
            var cashFlow = new CashFlow { CashFlowId = 1 };
            var protocol = new Protocol();

            _cashFlowRepositoryMock
                .Setup(r => r.AddCashFlowAsync(cashFlow))
                .ReturnsAsync(1);

            _protocolRepositoryMock
                .Setup(r => r.UpdateCashFlowIdAsync(protocol))
                .ReturnsAsync(true);

            // Act
            var result = await _service.AddCashFlowAsync(cashFlow, protocol);

            // Assert
            Assert.False(result.IsNotSuccess());
            Assert.Null(result.Message);
            Assert.Equal(0, result.StatusCode);

            _cashFlowRepositoryMock.Verify(r => r.AddCashFlowAsync(cashFlow), Times.Once);
            _protocolRepositoryMock.Verify(r => r.UpdateCashFlowIdAsync(protocol), Times.Once);
        }

        [Fact]
        public async Task AddCashFlowAsync_ShouldReturnDbError_WhenProtocolUpdateFails()
        {
            // Arrange
            var cashFlow = new CashFlow { CashFlowId = 1 };
            var protocol = new Protocol();

            _cashFlowRepositoryMock
                .Setup(r => r.AddCashFlowAsync(cashFlow))
                .ReturnsAsync(1);

            _protocolRepositoryMock
                .Setup(r => r.UpdateCashFlowIdAsync(protocol))
                .ReturnsAsync(false);

            // Act
            var result = await _service.AddCashFlowAsync(cashFlow, protocol);

            // Assert
            Assert.True(result.IsNotSuccess());
            Assert.Equal(ErrorMessage.DbError, result.Message);
            Assert.Equal(500, result.StatusCode);

            _cashFlowRepositoryMock.Verify(r => r.AddCashFlowAsync(cashFlow), Times.Once);
            _protocolRepositoryMock.Verify(r => r.UpdateCashFlowIdAsync(protocol), Times.Once);
        }

        [Fact]
        public async Task AddCashFlowAsync_ShouldReturnSuccess_WhenProtocolIsNull()
        {
            // Arrange
            var cashFlow = new CashFlow { CashFlowId = 1 };

            _cashFlowRepositoryMock
                .Setup(r => r.AddCashFlowAsync(cashFlow))
                .ReturnsAsync(1);

            // Act
            var result = await _service.AddCashFlowAsync(cashFlow, null);

            // Assert
            Assert.False(result.IsNotSuccess());
            Assert.Null(result.Message);
            Assert.Equal(0, result.StatusCode);
            _cashFlowRepositoryMock.Verify(r => r.AddCashFlowAsync(cashFlow), Times.Once);
            _protocolRepositoryMock.Verify(r => r.UpdateCashFlowIdAsync(It.IsAny<Protocol>()), Times.Never);
        }
    }
}
